using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public class QueryHelper {
    private static QueryHelper? _instance;
    private Schema? _schema;
    private bool _schemaExistsOnDatabase;

    public static QueryHelper Instance =>
      _instance ??= new QueryHelper();

    public Schema Schema {
      get => _schema ??= Schema.Instance;
      set => _schema = value;
    }

    public TEntity Read<TEntity>(
      string? simpleKey,
      SessionBase session) where TEntity : EntityBase {
      return Read<TEntity>(simpleKey, null, session);
    }

    public TEntity Read<TEntity>(
      string? simpleKey,
      EntityBase? identifyingParent,
      SessionBase session) where TEntity : EntityBase {
      try {
        return Read(
          CreateKeyPredicate<TEntity>(simpleKey, identifyingParent),
          session);
      } catch (InvalidOperationException exception) {
        // with Message "Sequence contains no matching element"
        throw CreateKeyNotFoundException(exception);
      }

      Exception CreateKeyNotFoundException(Exception innerException) {
        var writer = new StringWriter();
        writer.Write($"A {typeof(TEntity).Name} with SimpleKey '{simpleKey}' ");
        if (identifyingParent != null) {
          writer.Write(
            "and IdentifyingParent (presumed to be of EntityType " +
            $"{identifyingParent.EntityType.Name}) '{identifyingParent.Key}' ");
        }
        writer.Write("cannot be found");
        return new ConstraintException(writer.ToString(), innerException);
      }
    }

    public TEntity Read<TEntity>(
      Func<TEntity, bool> predicate,
      SessionBase session) where TEntity : EntityBase {
      var root = FetchRoot<TEntity>(session);
      CheckRootHasBeenFound(root);
      var entities = root!.Values;
      return entities.First(predicate);
      // return session.AllObjects<TEntity>().First(predicate);
    }

    public TEntity? Find<TEntity>(
      string? simpleKey,
      SessionBase session) where TEntity : EntityBase {
      return Find<TEntity>(simpleKey, null, session);
    }

    public TEntity? Find<TEntity>(
      string? simpleKey,
      EntityBase? identifyingParent,
      SessionBase session) where TEntity : EntityBase {
      return Find(
        CreateKeyPredicate<TEntity>(simpleKey, identifyingParent),
        session);
    }

    public TEntity? Find<TEntity>(
      Func<TEntity, bool> predicate,
      SessionBase session) where TEntity : EntityBase {
      var root = FindRoot<TEntity>(session);
      var entities = root?.Values;
      // var entities = session.AllObjects<TEntity>();
      return entities?.FirstOrDefault(predicate);
    }

    public SortedEntityCollection<TEntity>? FindRoot<TEntity>(SessionBase session)
      where TEntity : EntityBase {
      return FindRoot(typeof(TEntity), session) as SortedEntityCollection<TEntity>;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive) but a different object identifier from the one specified,
    ///   if found, otherwise a null reference.
    /// </summary>
    internal EntityBase? FindDuplicateSimpleKey(Type entityType,
      Oid oid, string? simpleKey, SessionBase session) {
      var entity = FindTopLevelEntity(entityType, simpleKey, session);
      return entity != null && !entity.Oid.Equals(oid) ? entity : null;
    }

    /// <summary>
    ///   Returns an object the specified generic type, of which there is only expected
    ///   to be one, if found, otherwise a null reference.
    /// </summary>
    internal TPersistable? FindSingleton<TPersistable>(SessionBase session)
      where TPersistable : OptimizedPersistable {
      return FindSingleton(typeof(TPersistable), session) as TPersistable;
    }

    [ExcludeFromCodeCoverage]
    private static void CheckRootHasBeenFound<TEntity>(
      SortedEntityCollection<TEntity>? root)
      where TEntity : EntityBase {
      if (root == null) {
        throw new InvalidOperationException(
          $"Cannot find the root collection for {typeof(TEntity).Name}s.");
      }
    }

    private static Func<TEntity, bool> CreateKeyPredicate<TEntity>(
      string? simpleKey,
      EntityBase? identifyingParent)
      where TEntity : EntityBase {
      return
        entity => entity.SimpleKey == simpleKey &&
                  (entity.IdentifyingParent == null &&
                   identifyingParent == null ||
                   entity.IdentifyingParent != null &&
                   entity.IdentifyingParent
                     .Equals(identifyingParent));
    }

    /// <summary>
    ///   Fetches an object the specified type, of which there is only expected to be
    ///   one, if found, otherwise a null reference.
    /// </summary>
    private static IOptimizedPersistable? FetchSingleton(
      Type persistableType, SessionBase session) {
      return session.Open(
        session.DatabaseNumberOf(persistableType),
        // Why page number 2? I don't know, but it works for fetching singleton
        // objects. See 'Looking up objects' in the VelocityDB manual.
        // ReSharper disable once ArgumentsStyleNamedExpression
        2, 1, update: session.InUpdateTransaction);
    }

    private SortedEntityCollection<TEntity>? FetchRoot<TEntity>(
      SessionBase session) where TEntity : EntityBase {
      return
        FetchSingleton(Schema.RootTypes[typeof(TEntity)], session) as
          SortedEntityCollection<TEntity>;
    }

    private IEnumerable FetchEntities(Type entityType, SessionBase session) {
      var root = FindRoot(entityType, session);
      return root?.Values ?? new List<IEntity>();
    }

    public ISortedEntityCollection? FindRoot(Type entityType, SessionBase session) {
      return FindSingleton(Schema.RootTypes[entityType], session) as
        ISortedEntityCollection;
    }

    /// <summary>
    ///   Returns an object the specified type, of which there is only expected to be
    ///   one, if found, otherwise a null reference.
    /// </summary>
    private IOptimizedPersistable? FindSingleton(Type type, SessionBase session) {
      return SchemaExistsOnDatabase(session)
        ? FetchSingleton(type, session)
        : null;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive), if found, otherwise a null reference.
    /// </summary>
    private EntityBase? FindTopLevelEntity(Type entityType,
      string? simpleKey, SessionBase session) {
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      var entities = FetchEntities(entityType, session);
      return (from EntityBase e in entities
        where string.Compare(e.SimpleKey, simpleKey,
          StringComparison.OrdinalIgnoreCase) == 0
        select e).FirstOrDefault();
    }

    private bool SchemaExistsOnDatabase(SessionBase session) {
      bool result;
      if (_schemaExistsOnDatabase) {
        result = true;
      } else {
        result = session.ContainsDatabase(session.DatabaseLocations.First(), 1);
        _schemaExistsOnDatabase = result;
      }
      return result;
    }
  }
}