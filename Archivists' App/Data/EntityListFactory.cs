using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   A facility for the instantiation of EntityList types.
  /// </summary>
  public static class EntityListFactory<T> {
    static EntityListFactory() {
      Types = CreateTypeDictionary();
    }

    /// <summary>
    ///   Gets a sorted dictionary of Entity or EntityList types,
    ///   with the Entity name as the key
    ///   and the type as the value.
    /// </summary>
    [NotNull]
    // ReSharper disable once StaticMemberInGenericType
    public static SortedDictionary<string, Type> Types { get; }

    /// <overloads>
    ///   Creates an instance of an Entity or EntityList type.
    /// </overloads>
    /// <summary>
    ///   Creates an instance of an Entity or EntityList type, specifying
    ///   the name of the table whose data is to be listed
    ///   and optionally specifying constructor arguments.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table whose data is to be listed.
    /// </param>
    /// <param name="args">
    ///   An array of arguments that match in number, order, and type the parameters
    ///   of the constructor to invoke. If args is an empty array or null, the constructor
    ///   that takes no parameters (the default constructor) is invoked.
    /// </param>
    /// <returns>
    ///   The new instance.
    /// </returns>
    [NotNull]
    public static T Create(string tableName, params object[] args) {
      if (!Types.ContainsKey(tableName)) {
        throw new ArgumentOutOfRangeException(
          "Entity type " + tableName + " is not supported.");
      }
      return Create(Types[tableName], args);
    }

    /// <summary>
    ///   Creates an instance of the specified type
    ///   and optionally specifying constructor arguments.
    /// </summary>
    /// <param name="type">
    ///   The type of object to be created.
    /// </param>
    /// <param name="args">
    ///   An array of arguments that match in number, order, and type the parameters
    ///   of the constructor to invoke. If args is an empty array or null, the constructor
    ///   that takes no parameters (the default constructor) is invoked.
    /// </param>
    /// <returns>
    ///   The new instance.
    /// </returns>
    [NotNull]
    internal static T Create(Type type, params object[] args) {
      try {
        return (T)Activator.CreateInstance(
          type, args);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
    }

    /// <summary>
    ///   Creates a sorted dictionary of Entity or EntityList types,
    ///   with the Entity name as the key
    ///   and the type as the value.
    /// </summary>
    /// <returns>
    ///   The sorted dictionary created.
    /// </returns>
    [NotNull]
    private static SortedDictionary<string, Type> CreateTypeDictionary() {
      var result = new SortedDictionary<string, Type>();
      // TODO Add entity list types
      return result;
    }
  } //End of class
} //End of namespace