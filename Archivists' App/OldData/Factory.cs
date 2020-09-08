using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SoundExplorers.OldData {
  /// <summary>
  ///   A facility for the instantiation of types,
  ///   which, in the case of Entity or EntityList types,
  ///   can be specified by table name.
  /// </summary>
  public static class Factory<T> {
    private static SortedDictionary<string, Type> _types;

    /// <summary>
    ///   Gets a sorted dictionary of Entity or EntityList types,
    ///   with the Entity name as the key
    ///   and the type as the value.
    /// </summary>
    [NotNull]
    public static SortedDictionary<string, Type> Types {
      get {
        if (_types == null) {
          _types = CreateTypeDictionary();
        }
        return _types;
      }
    }

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
    public static T Create(Type type, params object[] args) {
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
      if (!typeof(T).GetInterfaces().Contains(typeof(IEntityColumnContainer))) {
        throw new NotSupportedException(
          "Finding subtypes by name is not supported for type "
          + typeof(T).Name + ".");
      }
      var result = new SortedDictionary<string, Type>();
      foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
        if (typeof(T).IsAssignableFrom(type)
            && !type.Name.Contains("Entity")) {
          result.Add(type.Name.Replace("List", string.Empty), type);
        }
      } // End of foreach
      return result;
    }
  } //End of class
} //End of namespace