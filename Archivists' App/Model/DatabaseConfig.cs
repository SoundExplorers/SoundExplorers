using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Database configuration parameters that are specified via an XML file.
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
    // ReSharper disable once MemberCanBePrivate.Global
    internal string ConfigFilePath {
      get => _configFilePath ??
             Global.GetApplicationFolderPath() +
             Path.DirectorySeparatorChar + "DatabaseConfig.xml";
      // ReSharper disable once UnusedMember.Global
      set => _configFilePath = value;
    }

    private XmlWriter XmlWriter { get; set; }

    [Description(@"Database folder path. Example: C:\Folder\Subfolder")]
    public string DatabaseFolderPath { get; private set; }

    public void Load() {
      if (File.Exists(ConfigFilePath)) {
        Data = LoadData();
        foreach (var parameterProperty in GetType().GetProperties()) {
          SetParameterPropertyValue(parameterProperty);
        }
      } else {
        DatabaseFolderPath = DefaultDatabaseFolderPath;
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
        WriteParametersToXml();
        // Write the close tag for the root element.
        XmlWriter.WriteEndElement();
        XmlWriter.WriteEndDocument();
        XmlWriter.Flush();
      }
    }

    private XmlWriter CreateXmlWriter() {
      try {
        return XmlWriter.Create(
          ConfigFilePath,
          new XmlWriterSettings {Indent = true});
      } catch (Exception ex) {
        throw Global.CreateFileException(ex, "Database configuration file",
          ConfigFilePath);
      }
    }

    private string GetPropertyDescription(string name) {
      return
        GetType().GetProperty(name)?
          .GetCustomAttribute<DescriptionAttribute>().Description;
    }

    private XElement LoadData() {
      try {
        return XElement.Load(ConfigFilePath);
      } catch (XmlException ex) {
        throw new ApplicationException(
          "The following XML error was found in database configuration file "
          + ConfigFilePath + ":" + Environment.NewLine
          + ex.Message);
      } catch (Exception ex) {
        throw Global.CreateFileException(ex, "Database configuration file",
          ConfigFilePath);
      }
    }

    private void SetParameterPropertyValue(PropertyInfo property) {
      var element = Data.Element(property.Name);
      if (element == null) {
        throw new ApplicationException(
          "The " + property.Name + " tag was not found in database configuration file "
          + ConfigFilePath + ".");
      }
      try {
        property.SetValue(
          this,
          Convert.ChangeType(
            element.Value.Trim(), property.PropertyType));
      } catch (FormatException) {
        throw new ApplicationException(
          "The " + property.Name + " parameter value '"
          + element.Value
          + "' found in database configuration file "
          + Path.GetFileName(ConfigFilePath) + " is not properly formatted.");
      }
    }

    private void WriteParametersToXml() {
      WriteCommentedElement(nameof(DatabaseFolderPath), DatabaseFolderPath);
    }

    private void WriteCommentedElement(
      string name,
      string value) {
      XmlWriter.WriteComment(GetPropertyDescription(name));
      XmlWriter.WriteElementString(name, value);
    }
  }
}