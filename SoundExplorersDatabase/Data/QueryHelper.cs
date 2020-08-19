using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
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
    internal static QueryHelper Instance =>
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
    ///   Returns an entity of the specified type with the specified SimpleKey
    ///  (case-insensitive), if found, otherwise a null reference.
    /// </summary>
    [CanBeNull]
    internal EntityBase FindByType([NotNull] Type entityType, [CanBeNull] string simpleKey,
      SessionBase session) {
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      var allObjectsConstructedMethod =
        AllObjectsGenericMethod.MakeGenericMethod(entityType);
      var entities = (IEnumerable)allObjectsConstructedMethod.Invoke(session,
        new object[] {true, true});
      return (from EntityBase e in entities
        where string.Compare(e.SimpleKey, simpleKey,
          StringComparison.OrdinalIgnoreCase) == 0
        select e).FirstOrDefault();
    }

    // /// <summary>
    // ///   Returns an entity, if found, with the same SimpleKey (case-insensitive)
    // ///   as the specified entity, otherwise a null reference.
    // /// </summary>
    // [CanBeNull]
    // internal EntityBase FindWithSameSimpleKey([NotNull] EntityBase entity,
    //   SessionBase session) {
    //   if (!SchemaExistsOnDatabase(session)) {
    //     return null;
    //   }
    //   var allObjectsConstructedMethod =
    //     AllObjectsGenericMethod.MakeGenericMethod(entity.EntityType);
    //   var entities = (IEnumerable)allObjectsConstructedMethod.Invoke(session,
    //     new object[] {true, true});
    //   return (from EntityBase e in entities
    //     where string.Compare(e.SimpleKey, entity.SimpleKey,
    //       StringComparison.OrdinalIgnoreCase) == 0
    //     select e).FirstOrDefault();
    // }

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
      return Read(
        CreateKeyPredicate<TEntity>(simpleKey, identifyingParent),
        session);
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