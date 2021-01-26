using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public static class Global {
    /// <summary>
    ///   Gets the format in which dates are to be shown on the grid.
    /// </summary>
    public const string DateFormat = "dd MMM yyyy";

    public static SessionBase Session { get; set; } = null!;

    /// <summary>
    ///   Creates an instance of the specified entity list type.
    /// </summary>
    /// <param name="type">
    ///   The type of entity list to be created.
    /// </param>
    public static IEntityList CreateEntityList(Type type) {
      try {
        return (IEntityList)Activator.CreateInstance(type)!
               ?? throw new InvalidOperationException(
                 "In Global.CreateEntityList, cannot create IEntityList.");
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
    public static SortedDictionary<string, Type> CreateEntityListTypeDictionary() {
      return new() {
        {nameof(Act), typeof(ActList)},
        {nameof(Event), typeof(EventList)},
        {nameof(EventType), typeof(EventTypeList)},
        {nameof(Genre), typeof(GenreList)},
        {nameof(Location), typeof(LocationList)},
        {nameof(Newsletter), typeof(NewsletterList)},
        {nameof(Series), typeof(SeriesList)},
        {nameof(Set), typeof(SetList)}
      };
    }

    public static Exception CreateFileException(
      Exception exception,
      string pathDescription,
      string path) {
      switch (exception) {
        case IOException _:
        case UnauthorizedAccessException _:
          return new ApplicationException(
            $"{pathDescription}:{Environment.NewLine}{exception.Message}");
        case NotSupportedException _:
          return new ApplicationException(
            pathDescription + " \""
                            + path
                            + "\" is not a properly formatted file path.");
        default:
          return exception;
      }
    }

    [ExcludeFromCodeCoverage]
    public static string GetApplicationFolderPath() {
      return Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
    }

    [ExcludeFromCodeCoverage]
    public static string GetProductName() {
      var entryAssembly =
        Assembly.GetEntryAssembly() ??
        throw new NullReferenceException(
          "In Global.GetProductName, cannot find entry assembly.");
      return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(entryAssembly,
        typeof(AssemblyProductAttribute), false)!)!.Product;
    }
  }
}