using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace SoundExplorers.Model; 

/// <summary>
///   Database configuration parameter properties that are specified via an XML file.
/// </summary>
/// <remarks>
///   The code for this class is based on the Parameters class of
///   the KiwiSaverTax application.
///   Refer to that again if this class needs to get more complicated.
/// </remarks>
public class DatabaseConfig : IDatabaseConfig {
  /// <summary>
  ///   TODO Replace hard coded DefaultDatabaseFolderPath value to support multiple developers.
  /// </summary>
  internal const string DefaultDatabaseFolderPath =
    @"D:\Simon\OneDrive\Documents\Software\Sound Explorers Audio Archive\Database";

  internal const string InsertDatabaseFolderPathHere =
    "Insert database folder path here";

  private const string ForDeveloperUseOnly = "For developer use only";

  private readonly string? _configFilePath;
  private XElement Data { get; set; } = null!;
  private XmlWriter XmlWriter { get; set; } = null!;

  [Description(
    "Path of the licence file for the VelocityDB object-oriented database management system.")]
  public string VelocityDbLicenceFilePath { get; protected init; } = null!;

  internal bool HasDatabaseFolderPathBeenSpecified =>
    DatabaseFolderPath != InsertDatabaseFolderPathHere && 
    !string.IsNullOrWhiteSpace(DatabaseFolderPath);

#if DEBUG
    internal bool HasLicenceFilePathBeenSpecified =>
      VelocityDbLicenceFilePath != ForDeveloperUseOnly && 
      !string.IsNullOrWhiteSpace(VelocityDbLicenceFilePath);
#endif

  /// <summary>
  ///   Gets or sets the path of the database configuration file.
  ///   The setter should only be required for testing.
  /// </summary>
  public string ConfigFilePath {
    get => _configFilePath ??
           Path.Combine(Global.GetApplicationFolderPath(), "DatabaseConfig.xml");
    protected init => _configFilePath = value;
  }

  [Description(@"Database folder path. Example: C:\Folder\Subfolder")]
  public string DatabaseFolderPath { get; protected set; } = null!;

  /// <summary>
  ///   Loads the data from the config file, if it already exists in the application
  ///   folder. Otherwise creates a new one and throws an exception that will be
  ///   handled to prompt the user to edit the file to specify the database folder
  ///   path.
  /// </summary>
  /// <remarks>
  ///   Creating a new config file will not work in production, unless the user
  ///   happened to have run the program as administrator. So instead the installer
  ///   needs to place a new config file into the application folder unless there is
  ///   one there already.
  /// </remarks>
  public void Load() {
    if (File.Exists(ConfigFilePath)) {
      Data = LoadData();
      foreach (var property in GetType().GetProperties()) {
        if (property.Name != "ConfigFilePath") {
          SetPropertyValue(property);
        }
      }
    } else {
      DatabaseFolderPath = GetDatabaseFolderPath();
      CreateConfigFile();
      throw CreateDatabaseFolderNotSpecifiedException();
    }
  }

  internal ApplicationException CreateDatabaseFolderNotSpecifiedException() {
    return new ApplicationException(
      $"Please edit database configuration file '{ConfigFilePath}' "
      + "to specify the database folder path. " +
      "(Administrator access will be required.)");
  }

  private void CreateConfigFile() {
    using (XmlWriter = CreateXmlWriter()) {
      XmlWriter.WriteComment("Database configuration parameters");
      // Write the root element.
      XmlWriter.WriteStartElement(nameof(DatabaseConfig));
      WritePropertiesToXml();
      // Write the close tag for the root element.
      XmlWriter.WriteEndElement();
      XmlWriter.WriteEndDocument();
      XmlWriter.Flush();
    }
  }

  private XmlWriter CreateXmlWriter() {
    return DoGetConfigFileObject(() => XmlWriter.Create(
      ConfigFilePath,
      new XmlWriterSettings {Indent = true}));
  }

  [ExcludeFromCodeCoverage]
  private ApplicationException CreateXmlElementValueFormatException(
    string name, string? value) {
    return new ApplicationException(
      "The " + name + " property value '"
      + value
      + "' found in database configuration file "
      + Path.GetFileName(ConfigFilePath) + " is not properly formatted.");
  }

  [ExcludeFromCodeCoverage]
  private TResult DoGetConfigFileObject<TResult>(Func<TResult> function) {
    try {
      return function.Invoke();
    } catch (Exception exception) {
      throw Global.CreateFileException(exception, "Database configuration file",
        ConfigFilePath);
    }
  }

  private string GetPropertyDescription(string name) {
    return
      GetType().GetProperty(name)!.GetCustomAttribute<DescriptionAttribute>()!
        .Description;
  }

  private XElement LoadData() {
    return DoGetConfigFileObject(() => {
      try {
        return XElement.Load(ConfigFilePath);
      } catch (XmlException ex) {
        throw new ApplicationException(
          "The following XML error was found in database configuration file "
          + ConfigFilePath + ":" + Environment.NewLine
          + ex.Message);
      }
    });
  }

  [ExcludeFromCodeCoverage]
  protected virtual string GetDatabaseFolderPath() {
#if DEBUG
      return DefaultDatabaseFolderPath;
#else // Release build
    return InsertDatabaseFolderPathHere;
#endif
  }

  private void SetPropertyValue(PropertyInfo property) {
    var element = Data.Element(property.Name);
    if (element == null) {
      throw new ApplicationException(
        "The " + property.Name + " tag was not found in database configuration file "
        + ConfigFilePath + ".");
    }
    SetPropertyValueFromXmlElement(property, element);
  }

  [ExcludeFromCodeCoverage]
  private void SetPropertyValueFromXmlElement(
    PropertyInfo property, XElement element) {
    try {
      property.SetValue(
        this,
        Convert.ChangeType(
          element.Value.Trim(), property.PropertyType));
    } catch (FormatException) {
      throw CreateXmlElementValueFormatException(property.Name, element.Value);
    }
  }

  private void WritePropertiesToXml() {
    WriteCommentedElement(nameof(DatabaseFolderPath), DatabaseFolderPath);
    WriteCommentedElement(nameof(VelocityDbLicenceFilePath), ForDeveloperUseOnly);
  }

  private void WriteCommentedElement(string name, string value) {
    XmlWriter.WriteComment(GetPropertyDescription(name));
    XmlWriter.WriteElementString(name, value);
  }
}