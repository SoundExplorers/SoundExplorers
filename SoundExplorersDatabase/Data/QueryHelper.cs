﻿using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public static class QueryHelper {
    [CanBeNull]
    public static TPersistable Find<TPersistable>(
      [CanBeNull] object key,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      if (!Schema.Instance.ExistsOnDatabase(session)) {
        return null;
      }
      return session.AllObjects<TPersistable>()
        .FirstOrDefault(persistable => persistable.Key.Equals(key));
    }
    
    [NotNull]
    public static TPersistable Read<TPersistable>(
      [CanBeNull] object key,
      [NotNull] SessionBase session) where TPersistable : RelativeBase {
      return session.AllObjects<TPersistable>()
        .First(persistable => persistable.Key.Equals(key));
    }
  }
}