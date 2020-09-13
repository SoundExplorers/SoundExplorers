using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public static class Global {
    public static SessionBase Session { get; set; }
    
    /// <summary>
    ///   Creates an instance of the specified entity list type.
    /// </summary>
    /// <param name="type">
    ///   The type of entity list to be created.
    /// </param>
    [NotNull]
    public static IEntityList CreateEntityList([NotNull] Type type) {
      try {
        return (IEntityList)Activator.CreateInstance(type);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }

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