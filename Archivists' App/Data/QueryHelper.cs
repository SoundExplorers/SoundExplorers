using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public class QueryHelper {
    private static QueryHelper _instance;
    private bool _schemaExistsOnDatabase;

    static QueryHelper() {
      AllObjectsGenericMethod =
        typeof(SessionBase).GetMethod("AllObjects") ??
        throw new NullReferenceException("Cannot find generic method.");
    }

    [NotNull] private static MethodInfo AllObjectsGenericMethod { get; }

    [NotNull]
    public static QueryHelper Instance =>
      _instance ?? (_instance = new QueryHelper());

    [NotNull]
    private static Func<TEntity, bool> CreateKeyPredicate<TEntity>(
      [CanBeNull] string simpleKey,
      [CanBeNull] EntityBase identifyingParent)
      where TEntity : EntityBase {
      return
        entity => entity.SimpleKey == simpleKey &&
                  (entity.IdentifyingParent == null &&
                   identifyingParent == null ||
                   entity.IdentifyingParent != null &&
                   entity.IdentifyingParent
                     .Equals(identifyingParent));
    }

    [CanBeNull]
    public TEntity Find<TEntity>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TEntity : EntityBase {
      return Find<TEntity>(simpleKey, null, session);
    }

    [CanBeNull]
    public TEntity Find<TEntity>(
      [CanBeNull] string simpleKey,
      [CanBeNull] EntityBase identifyingParent,
      [NotNull] SessionBase session) where TEntity : EntityBase {
      return Find(
        CreateKeyPredicate<TEntity>(simpleKey, identifyingParent),
        session);
    }

    [CanBeNull]
    public TEntity Find<TEntity>(
      [NotNull] Func<TEntity, bool> predicate,
      [NotNull] SessionBase session) where TEntity : EntityBase {
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      return session.AllObjects<TEntity>()
        .FirstOrDefault(predicate);
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive) but a different object identifier from the one specified,
    ///   if found, otherwise a null reference.
    /// </summary>
    [CanBeNull]
    internal EntityBase FindDuplicateSimpleKey([NotNull] Type entityType,
      Oid oid, [CanBeNull] string simpleKey, SessionBase session) {
      var entity = FindTopLevelEntity(entityType, simpleKey, session);
      return entity != null && !entity.Oid.Equals(oid) ? entity : null;
    }

    [NotNull]
    public static IEnumerable FetchEntities([NotNull] Type entityType, 
      SessionBase session) {
      // This complicated rigmarole is required to allow
      // SessionBase.AllObjects<T> to be invoked with a an ordinary parameter
      // instead of the type parameter T.
      // Unfortunately VelocityDB does not provide a non-generic alternative.
      var allObjectsConstructedMethod =
        AllObjectsGenericMethod.MakeGenericMethod(entityType);
      return (IEnumerable)allObjectsConstructedMethod.Invoke(session,
        new object[] {true, true});
    }

    /// <summary>
    ///   Returns a top-level entity of the specified type with the specified SimpleKey
    ///   (case-insensitive), if found, otherwise a null reference.
    /// </summary>
    [CanBeNull]
    private EntityBase FindTopLevelEntity([NotNull] Type entityType,
      [CanBeNull] string simpleKey, SessionBase session) {
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

    [NotNull]
    public static TEntity Read<TEntity>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TEntity : EntityBase {
      return Read<TEntity>(simpleKey, null, session);
    }

    [NotNull]
    public static TEntity Read<TEntity>(
      [CanBeNull] string simpleKey,
      [CanBeNull] EntityBase identifyingParent,
      [NotNull] SessionBase session) where TEntity : EntityBase {
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

    [NotNull]
    public static TEntity Read<TEntity>(
      [NotNull] Func<TEntity, bool> predicate,
      [NotNull] SessionBase session) where TEntity : EntityBase {
      return session.AllObjects<TEntity>()
        .First(predicate);
    }

    public bool SchemaExistsOnDatabase([NotNull] SessionBase session) {
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