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
    private static Func<TPersistable, bool> CreateKeyPredicate<TPersistable>(
      [CanBeNull] string simpleKey,
      [CanBeNull] RelativeBase identifyingParent)
      where TPersistable : RelativeBase {
      return
        persistable => persistable.SimpleKey == simpleKey &&
                       (persistable.IdentifyingParent == null &&
                        identifyingParent == null ||
                        persistable.IdentifyingParent != null &&
                        persistable.IdentifyingParent
                          .Equals(identifyingParent));
    }

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Find<TPersistable>(simpleKey, null, session);
    }

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [CanBeNull] string simpleKey,
      [CanBeNull] RelativeBase identifyingParent,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Find(
        CreateKeyPredicate<TPersistable>(simpleKey, identifyingParent),
        session);
    }

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [NotNull] Func<TPersistable, bool> predicate,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      return session.AllObjects<TPersistable>()
        .FirstOrDefault(predicate);
    }

    [CanBeNull]
    internal RelativeBase FindWithSameSimpleKey([NotNull] RelativeBase relative,
      SessionBase session) {
      if (!SchemaExistsOnDatabase(session)) {
        return null;
      }
      var allObjectsConstructedMethod =
        AllObjectsGenericMethod.MakeGenericMethod(relative.PersistableType);
      var relatives = (IEnumerable)allObjectsConstructedMethod.Invoke(session,
        new object[] {true, true});
      return (from RelativeBase r in relatives
        where r.SimpleKey == relative.SimpleKey
        select r).FirstOrDefault();
    }

    [NotNull]
    public static TPersistable Read<TPersistable>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Read<TPersistable>(simpleKey, null, session);
    }

    [NotNull]
    public static TPersistable Read<TPersistable>(
      [CanBeNull] string simpleKey,
      [CanBeNull] RelativeBase identifyingParent,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Read(
        CreateKeyPredicate<TPersistable>(simpleKey, identifyingParent),
        session);
    }

    [NotNull]
    public static TPersistable Read<TPersistable>(
      [NotNull] Func<TPersistable, bool> predicate,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return session.AllObjects<TPersistable>()
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