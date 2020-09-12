using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   A facility for the instantiation of entity list types.
  /// </summary>
  public static class EntityListFactory {
    /// <summary>
    ///   Creates an instance of the specified entity list type.
    /// </summary>
    /// <param name="type">
    ///   The type of entity list to be created.
    /// </param>
    [NotNull]
    public static IEntityList Create([NotNull] Type type) {
      try {
        return (IEntityList)Activator.CreateInstance(type);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }
  } //End of class
} //End of namespace