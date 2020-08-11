using System;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class QueryHelper {
    private static QueryHelper _instance;
    private bool _schemaExistsOnDatabase;

    internal static QueryHelper Instance =>
      _instance ?? (_instance = new QueryHelper());

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Find<TPersistable>(
        persistable => persistable.Key == new Key(simpleKey, null), session);
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

    [NotNull]
    public TPersistable Read<TPersistable>(
      [CanBeNull] string simpleKey,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Read<TPersistable>(
        persistable => persistable.Key == new Key(simpleKey, null), session);
    }

    [NotNull]
    public TPersistable Read<TPersistable>(
      [CanBeNull] string simpleKey,
      [CanBeNull] RelativeBase identifyingParent,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Read<TPersistable>(
        persistable => persistable.Key == new Key(simpleKey, identifyingParent),
        session);
    }

    [NotNull]
    public TPersistable Read<TPersistable>(
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