using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class DatabaseConnectionTests {
    [SetUp]
    public void Setup() {
      ConfigFilePath = TestSession.DatabaseParentFolderPath +
                       Path.DirectorySeparatorChar + "DatabaseConfig.xml";
      DatabaseFolderPath = TestSession.DatabaseParentFolderPath +
                           Path.DirectorySeparatorChar +
                           "Connection Test Database";
      TearDown(); // Delete the config file and database folder if they exist.
      Connection = new TestDatabaseConnection(ConfigFilePath, DatabaseFolderPath);
    }

    [TearDown]
    public void TearDown() {
      if (File.Exists(ConfigFilePath)) {
        File.Delete(ConfigFilePath);
      }
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private string ConfigFilePath { get; set; }
    private TestDatabaseConnection Connection { get; set; }
    private string DatabaseFolderPath { get; set; }

    [Test]
    public void TheTest() {
      // Neither the configuration file nor the database folder exist.
      try {
        Connection.Open();
        Assert.Fail(
          "Open should have thrown ApplicationException for missing configuration file.");
      } catch (ApplicationException exception) {
        Assert.IsTrue(
          exception.Message.StartsWith("Please edit database configuration file '"),
          "Missing configuration file message");
      }
      // The configuration file has been created and already contains
      // the database folder path we want to use.
      // So we don't actually need to edit the configuration file.  
      try {
        Connection.Open();
        Assert.Fail(
          "Open should have thrown ApplicationException for missing database folder.");
      } catch (ApplicationException exception) {
        Assert.IsTrue(
          exception.Message.StartsWith("Database folder '"),
          "Missing database folder message");
      }
    }
  }
}