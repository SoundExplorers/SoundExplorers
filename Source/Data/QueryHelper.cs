using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public class QueryHelper {
    private static QueryHelper? _instance;
    private bool _schemaExistsOnDatabase;

    static QueryHelper() {
      IndexGenericMethod = typeof(SessionBase).GetMethod(
        "Index", Array.Empty<Type>())!;
    }

    public static QueryHelper Instance =>
      _instance ??= new QueryHelper();

    private static MethodInfo IndexGenericMethod { get; }

    public static TEntity Read<TEntity>(
      string? simpleKey,
      SessionBase session) where TEntity : EntityBase {
      return Read<TEntity>(simpleKey, null, session);
    }

    public static TEntity Read<TEntity>(
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

    public static TEntity Read<TEntity>(
      Func<TEntity, bool> predicate,
      SessionBase session) where TEntity : EntityBase {
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
      return SchemaExistsOnDatabase(session)
        ? session.Index<TEntity>()?.FirstOrDefault(predicate)
        : null;
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive) but a different object identifier from the one specified,
    ///   if found, otherwise a null reference.
    /// </summary>
    internal EntityBase? FindDuplicateSimpleKey(Type entityType,
      Oid oid, string simpleKey, SessionBase session) {
      var entity = SchemaExistsOnDatabase(session)
        ? FindTopLevelEntity(entityType, simpleKey, session)
        : null;
      return entity != null && !entity.Oid.Equals(oid) ? entity : null;
    }

    /// <summary>
    ///   Returns an object the specified generic type, of which there is only expected
    ///   to be one, if found, otherwise a null reference.
    /// </summary>
    internal TPersistable? FindSingleton<TPersistable>(SessionBase session)
      where TPersistable : OptimizedPersistable {
      return SchemaExistsOnDatabase(session)
        ? session.Open(
          session.DatabaseNumberOf(typeof(TPersistable)),
          // Why page number 2? I don't know, but it works for fetching singleton
          // objects. See 'Looking up objects' in the VelocityDB manual.
          // ReSharper disable once ArgumentsStyleNamedExpression
          2, 1, update: session.InUpdateTransaction) as TPersistable
        : null;
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

    private static EntityBase? FindTopLevelEntity(Type entityType, string simpleKey,
      SessionBase session) {
      var indexConstructedMethod =
        IndexGenericMethod.MakeGenericMethod(entityType);
      return !(indexConstructedMethod.Invoke(session, null) is
        IEnumerable index)
        ? null
        : (from EntityBase entity in index
          where string.Compare(entity.SimpleKey, simpleKey,
            StringComparison.OrdinalIgnoreCase) == 0
          select entity).FirstOrDefault();
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