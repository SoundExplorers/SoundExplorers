using System;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class QueryHelper {
    public QueryHelper([NotNull] Schema schema) {
      Schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    private Schema Schema { get; }

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [CanBeNull] string key,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return Find<TPersistable>(
        persistable => persistable.Key == key, session);
    }

    [CanBeNull]
    public TPersistable Find<TPersistable>(
      [NotNull] Func<TPersistable, bool> predicate,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      if (!Schema.ExistsOnDatabase(session)) {
        return null;
      }
      return session.AllObjects<TPersistable>()
        .FirstOrDefault(predicate);
    }

    [NotNull]
    public TPersistable Read<TPersistable>(
      [CanBeNull] string key,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return session.AllObjects<TPersistable>()
        .First(persistable => persistable.Key == key);
    }
  }
}