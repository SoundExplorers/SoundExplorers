using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model; 

[TestFixture]
public class DatabaseConnectionTests {
  [SetUp]
  public void Setup() {
    ConfigFilePath = Path.Combine(
      TestSession.DatabaseParentFolderPath, "DatabaseConfig.xml");
    DatabaseFolderPath = Path.Combine(
      TestSession.DatabaseParentFolderPath, "Connection Test Database");
    TearDown(); // Delete the config file and database folder if they exist.
    Connection = new TestDatabaseConnection(ConfigFilePath, DatabaseFolderPath,
      new QueryHelper());
  }

  [TearDown]
  public void TearDown() {
    if (File.Exists(ConfigFilePath)) {
      File.Delete(ConfigFilePath);
    }
    TestSession.DeleteFolderIfExists(DatabaseFolderPath);
  }

  private string ConfigFilePath { get; set; } = null!;
  private TestDatabaseConnection Connection { get; set; } = null!;
  private string DatabaseFolderPath { get; set; } = null!;

  [Test]
  public void MainTest() {
    // Neither the configuration file nor the database folder exist.
    var exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for missing configuration file.");
    Assert.IsTrue(
      exception!.Message.StartsWith("Please edit database configuration file '"),
      "Missing configuration file message");
    // The configuration file has been created and already contains the database folder
    // path we want to use. So we don't actually need to edit the configuration file.  
    exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for missing database folder.");
    Assert.IsTrue(
      exception!.Message.StartsWith("Database folder '"),
      "Missing database folder message");
    Directory.CreateDirectory(DatabaseFolderPath);
#if DEBUG
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for unspecified licence file.");
      Assert.IsTrue(
        exception!.Message.StartsWith("Please specify the path "),
        "Unspecified licence file message");
      UpdateVelocityDbLicenceFilePath(false);
      Assert.AreEqual("For developer use only",
        Connection.DatabaseConfig.VelocityDbLicenceFilePath,
        "Unspecified VelocityDbLicenceFilePath");
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for non-existent licence file.");
      Assert.IsTrue(
        exception!.Message.StartsWith("VelocityDB licence file '"),
        "Non-existent licence file message");
      UpdateVelocityDbLicenceFilePath(true);
#endif
    Connection.Open();
#if DEBUG
      Assert.AreEqual(TestSession.VelocityDbLicenceFilePath,
        Connection.DatabaseConfig.VelocityDbLicenceFilePath,
        "VelocityDbLicenceFilePath");
#else // Release build
    Assert.AreEqual("For developer use only",
      Connection.DatabaseConfig.VelocityDbLicenceFilePath, 
      "VelocityDbLicenceFilePath");
#endif
    Assert.AreEqual(DatabaseFolderPath.ToLower(), Global.Session.SystemDirectory,
      "SystemDirectory");
    Assert.AreEqual(Connection.ExpectedSchemaVersion, Schema.Instance.Version,
      "Version, change #1");
    Assert.AreEqual(SchemaUpgradeStatus.Complete, 
      Connection.SchemaUpgradeStatus, "SchemaUpgradeStatus after version change #1");
    ResetSchemaVersion(1);
    Connection.MockBackupManager.LastBackupDateTime = DateTime.Now.AddMonths(-1);
    Connection.Open();
    // The Schema already exists at an earlier version than expected. So it needs to be
    // upgraded. But we cannot do that yet, as the database has not been backed up
    // within the last day. Instead, we need to set a flag for the user to be forced to
    // back up the database and restart the application to complete the upgrade.
    Assert.AreEqual(1, Schema.Instance.Version, "Version when backup is required");
    Assert.AreEqual(SchemaUpgradeStatus.Pending, Connection.SchemaUpgradeStatus, 
      "SchemaUpgradeStatus backup is required");
    // Show that the user has just backed up the database, so we can proceed with the 
    // schema upgrade.
    Connection.MockBackupManager.LastBackupDateTime = DateTime.Now.AddMinutes(-1);
    // As we have just made the schema version out of date,
    // the entity types have to be registered again.
    // However, a copy of the licence file is now already on the database,
    // so we don't have to copy it again.
    Connection.Open();
    Assert.AreEqual(Connection.ExpectedSchemaVersion, Schema.Instance.Version,
      "Version, change #2");
    Assert.AreEqual(SchemaUpgradeStatus.Complete, Connection.SchemaUpgradeStatus, 
      "SchemaUpgradeStatus after version change #2");
    // The schema version is now up to date;
    Connection.Open();
    Assert.AreEqual(SchemaUpgradeStatus.None, Connection.SchemaUpgradeStatus, 
      "SchemaUpgradeStatus when the schema is up to date.");
    RemoveXmlElement();
    exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for missing XML element.");
    Assert.IsTrue(
      exception!.Message.Contains(
        " tag was not found in database configuration file "),
      "Missing XML element");
    MakeXmlError();
    exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for XML error.");
    Assert.IsTrue(
      exception!.Message.StartsWith(
        "The following XML error was found in database configuration file "),
      "XML error");
  }

  [Test]
  public void SimulateReleaseBuild() {
    Connection = new TestDatabaseConnection(ConfigFilePath,
      DatabaseConfig.InsertDatabaseFolderPathHere, new QueryHelper());
    // Neither the configuration file nor the database folder exist.
    var exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for missing configuration file.");
    Assert.IsTrue(
      exception!.Message.StartsWith("Please edit database configuration file '"),
      "Missing configuration file message");
    // The configuration file has been created but the database folder path has not
    // been specified.  If the user attempts to reload the application without having
    // edited the configuration file to specify the database folder path, the
    // application load should fail with the same error message as before.
    exception = Assert.Catch<ApplicationException>(
      () => Connection.Open(),
      "Open should have thrown ApplicationException for unspecified database folder path.");
    Assert.IsTrue(
      exception!.Message.StartsWith("Please edit database configuration file '"),
      "Unspecified database folder path file message");
  }

  private void MakeXmlError() {
    using var writer = new StreamWriter(ConfigFilePath);
    writer.Write("This is not an XML file.");
  }

  private void RemoveXmlElement() {
    string configText;
    using (var reader = new StreamReader(ConfigFilePath)) {
      configText = reader.ReadToEnd();
    }
    using (var writer = new StreamWriter(ConfigFilePath)) {
      writer.Write(configText.Replace(
        $"<DatabaseFolderPath>{DatabaseFolderPath}</DatabaseFolderPath>",
        string.Empty));
    }
  }

  private void ResetSchemaVersion(int toVersion) {
    var session = new SessionNoServer(DatabaseFolderPath);
    session.BeginUpdate();
    var schema = Schema.Find(QueryHelper.Instance, session)!;
    Assert.IsNotNull(schema, "schema");
    schema.Version = toVersion;
    session.Commit();
  }

#if DEBUG
    private void UpdateVelocityDbLicenceFilePath(bool withRealPath) {
      const string nonExistentPath = @"w:\xyz.xml";
      string configText;
      using (var reader = new StreamReader(ConfigFilePath)) {
        configText = reader.ReadToEnd();
      }
      using (var writer = new StreamWriter(ConfigFilePath)) {
        // For this to work, the method needs to be called first with withRealPath false
        // and then with withRealPath true;
        if (withRealPath) {
          writer.Write(configText.Replace(nonExistentPath,
            TestSession.VelocityDbLicenceFilePath));
        } else {
          writer.Write(configText.Replace("For developer use only",
            nonExistentPath));
        }
      }
    }
#endif
}