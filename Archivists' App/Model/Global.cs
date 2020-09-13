using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public static class Global {
    public static SessionBase Session { get; set; }

    /// <summary>
    ///   Creates a sorted dictionary of entity list types,
    ///   with the entity name as the key
    ///   and the type as the value.
    /// </summary>
    /// <returns>
    ///   The sorted dictionary created.
    /// </returns>
    [NotNull]
    public static SortedDictionary<string, Type> CreateEntityListTypeDictionary() {
      return new SortedDictionary<string, Type> {
        {string.Empty, null},
        {nameof(Event), typeof(EventList)},
        {nameof(Set), typeof(SetList)}
      };
    }
  }
}