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
      AllObjectsGenericMethod = typeof(SessionBase).GetMethod("AllObjects")!;
    }

    public static QueryHelper Instance =>
      _instance ??= new QueryHelper();

    private static MethodInfo AllObjectsGenericMethod { get; }

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
            $"and IdentifyingParent (presumed to be of EntityType {identifyingParent.EntityType.Name}) " +
            $"'{identifyingParent.Key}' ");
        }
        writer.Write("cannot be found");
        return new ConstraintException(writer.ToString(), innerException);
      }
    }

    public static TEntity Read<TEntity>(
      Func<TEntity, bool> predicate,
      SessionBase session) where TEntity : EntityBase {
      return session.AllObjects<TEntity>()
        .First(predicate);
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
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      var entities = session.AllObjects<TEntity>();
      return entities.FirstOrDefault(predicate);
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

    internal TPersistable? FindSingleton<TPersistable>(SessionBase session) 
      where TPersistable: OptimizedPersistable {
      return SchemaExistsOnDatabase(session)
        ? session.Open(
          session.DatabaseNumberOf(typeof(TPersistable)), 
          2, 1, session.InUpdateTransaction) as TPersistable 
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

    private static IEnumerable FetchEntities(Type entityType,
      SessionBase session) {
      // This complicated rigmarole is required to allow
      // SessionBase.AllObjects<T> to be invoked with a an ordinary parameter
      // instead of the type parameter T.
      // Unfortunately VelocityDB does not provide a non-generic alternative.
      var allObjectsConstructedMethod =
        AllObjectsGenericMethod.MakeGenericMethod(entityType);
      return (IEnumerable)allObjectsConstructedMethod.Invoke(session,
        new object[] {true, true})!;
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
      // IEnumerable entities;
      // try {
      //   entities = (IEnumerable)allObjectsConstructedMethod.Invoke(session,
      //     new object[] {true, true});
      // } catch (Exception exception) {
      //   Debug.WriteLine(exception);
      //   throw;
      // }
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