using System;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public class QueryHelper {
    private static QueryHelper? _instance;
    private Schema? _schema;
    private bool _schemaExistsOnDatabase;
    
    static QueryHelper() {
      IndexGenericMethod = typeof(SessionBase).GetMethod(
        "Index", Array.Empty<Type>())!;
    }

    public static QueryHelper Instance =>
      _instance ??= new QueryHelper();

    internal Schema Schema {
      get => _schema ??= Schema.Instance;
      set => _schema = value;
    }

    private static MethodInfo IndexGenericMethod { get; }

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

    public static TEntity Read2<TEntity>(
      string? simpleKey,
      SessionBase session) where TEntity : EntityBase2 {
      return Read2<TEntity>(simpleKey, null, session);
    }

    public static TEntity Read2<TEntity>(
      string? simpleKey,
      EntityBase2? identifyingParent,
      SessionBase session) where TEntity : EntityBase2 {
      try {
        return Read2(
          CreateKeyPredicate2<TEntity>(simpleKey, identifyingParent),
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

    public static TEntity Read2<TEntity>(
      Func<TEntity, bool> predicate,
      SessionBase session) where TEntity : EntityBase2 {
      var index = 
        session.Index<TEntity>() 
        ?? throw new InvalidOperationException(
          $"Cannot find {typeof(TEntity).Name} index.");
      return index.First(predicate);
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
      var root = FindRoot(typeof(TEntity), session) as SortedEntityCollection<TEntity>;
      var entities = root?.Values;
      return entities?.FirstOrDefault(predicate);
    }

    public ISortedEntityCollection? FindRoot(Type entityType, SessionBase session) {
      return FindSingleton(Schema.RootTypes[entityType], session) as
        ISortedEntityCollection;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive) but a different object identifier from the one specified,
    ///   if found, otherwise a null reference.
    /// </summary>
    internal EntityBase? FindDuplicateSimpleKey(Type entityType,
      Oid oid, string simpleKey, SessionBase session) {
      var entity = FindTopLevelEntity(entityType, simpleKey, session);
      return entity != null && !entity.Oid.Equals(oid) ? entity : null;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive) but a different object identifier from the one specified,
    ///   if found, otherwise a null reference.
    /// </summary>
    internal EntityBase2? FindDuplicateSimpleKey2(Type entityType,
      Oid oid, string simpleKey, SessionBase session) {
      var entity = FindTopLevelEntity2(entityType, simpleKey, session);
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

    private static Func<TEntity, bool> CreateKeyPredicate2<TEntity>(
      string? simpleKey,
      EntityBase2? identifyingParent)
      where TEntity : EntityBase2 {
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

    private EntityBase? FetchTopLevelEntity(Type entityType, string simpleKey, 
      SessionBase session) {
      var root = FindRoot(entityType, session);
      return root == null
        ? null :
        (from EntityBase entity in root.Values
        where string.Compare(entity.SimpleKey, simpleKey,
          StringComparison.OrdinalIgnoreCase) == 0
        select entity).FirstOrDefault();
    }

    private static EntityBase2? FetchTopLevelEntity2(Type entityType, string simpleKey, 
      SessionBase session) {
      var indexConstructedMethod =
        IndexGenericMethod.MakeGenericMethod(entityType);
      return !(indexConstructedMethod.Invoke(session, null) is 
        IEnumerable index)
        ? null :
        (from EntityBase2 entity in index
          where string.Compare(entity.SimpleKey, simpleKey,
            StringComparison.OrdinalIgnoreCase) == 0
          select entity).FirstOrDefault();
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
      string simpleKey, SessionBase session) {
      return SchemaExistsOnDatabase(session)
        ? FetchTopLevelEntity(entityType, simpleKey, session)
        : null;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive), if found, otherwise a null reference.
    /// </summary>
    private EntityBase2? FindTopLevelEntity2(Type entityType,
      string simpleKey, SessionBase session) {
      return SchemaExistsOnDatabase(session)
        ? FetchTopLevelEntity2(entityType, simpleKey, session)
        : null;
    }

    private bool SchemaExistsOnDatabase(SessionBase session) {
      bool result;
      if (_schemaExistsOnDatabase) {
        result = true;
      } else {
        result = session.ContainsDatabase(
          session.DatabaseLocations.First(), 1);
        _schemaExistsOnDatabase = result;
      }
      return result;
    }
  }
}