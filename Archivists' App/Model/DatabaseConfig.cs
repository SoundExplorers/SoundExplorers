using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Database configuration parameter properties that are specified via an XML file.
  /// </summary>
  /// <remarks>
  ///   The code for this class is based on the Parameters class of
  ///   the KiwiSaverTax application.
  ///   Refer to that again if this class needs to get more complicated.
  /// </remarks>
  public class DatabaseConfig {
    private const string DefaultDatabaseFolderPath =
      @"E:\Simon\OneDrive\Documents\Software\Sound Explorers Audio Archive\Database";

    private string _configFilePath;
    private XElement Data { get; set; }

    /// <summary>
    ///   Gets or sets the path of the database configuration file.
    ///   The setter should only be required for testing.
    /// </summary>
    protected internal string ConfigFilePath {
      get => _configFilePath ??
             Global.GetApplicationFolderPath() +
             Path.DirectorySeparatorChar + "DatabaseConfig.xml";
      protected set => _configFilePath = value;
    }

    private XmlWriter XmlWriter { get; set; }

    [Description(@"Database folder path. Example: C:\Folder\Subfolder")]
    public string DatabaseFolderPath { get; protected set; }

    [Description(
      "Path of the licence file for the VelocityDB object-oriented database management system.")]
    public string VelocityDbLicenceFilePath { get; protected set; }

    public void Load() {
      if (File.Exists(ConfigFilePath)) {
        Data = LoadData();
        foreach (var property in GetType().GetProperties()) {
          SetPropertyValue(property);
        }
      } else {
        DatabaseFolderPath = SetDatabaseFolderPath();
        CreateConfigFile();
        throw new ApplicationException(
          $"Please edit database configuration file '{ConfigFilePath}' "
          + "to specify the database folder path.");
      }
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

    [ExcludeFromCodeCoverage]
    private Exception CreateDatabaseConfigFileException([NotNull] Exception exception) {
      return Global.CreateFileException(exception, "Database configuration file",
        ConfigFilePath);
    }

    private XmlWriter CreateXmlWriter() {
      return DoGetConfigFileObject(() => XmlWriter.Create(
        ConfigFilePath,
        new XmlWriterSettings {Indent = true}));
    }

    [ExcludeFromCodeCoverage]
    private ApplicationException CreateXmlElementValueFormatException(
      [NotNull] string name, [CanBeNull] string value) {
      return new ApplicationException(
        "The " + name + " property value '"
        + value
        + "' found in database configuration file "
        + Path.GetFileName(ConfigFilePath) + " is not properly formatted.");
    }

    [ExcludeFromCodeCoverage]
    [NotNull]
    private TResult DoGetConfigFileObject<TResult>([NotNull] Func<TResult> function) {
      try {
        return function.Invoke();
      } catch (Exception exception) {
        throw CreateDatabaseConfigFileException(exception);
      }
    }

    private string GetPropertyDescription([NotNull] string name) {
      return
        GetType().GetProperty(name)?
          .GetCustomAttribute<DescriptionAttribute>().Description;
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
    protected virtual string SetDatabaseFolderPath() {
      return DefaultDatabaseFolderPath;
    }

    private void SetPropertyValue([NotNull] PropertyInfo property) {
      var element = Data.Element(property.Name);
      if (element == null) {
        throw new ApplicationException(
          "The " + property.Name + " tag was not found in database configuration file "
          + ConfigFilePath + ".");
      }
      SetPropertyValueFromXmlElement(property, element);
    }

    [ExcludeFromCodeCoverage]
    private void SetPropertyValueFromXmlElement([NotNull] PropertyInfo property,
      [NotNull] XElement element) {
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
      WriteCommentedElement(nameof(VelocityDbLicenceFilePath), "For developer use only");
    }

    private void WriteCommentedElement(
      [NotNull] string name,
      [NotNull] string value) {
      XmlWriter.WriteComment(GetPropertyDescription(name));
      XmlWriter.WriteElementString(name, value);
    }
  }
}