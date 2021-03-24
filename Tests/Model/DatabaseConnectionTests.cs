﻿using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using VelocityDb.Session;

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

    private string ConfigFilePath { get; set; } = null!;
    private TestDatabaseConnection Connection { get; set; } = null!;
    private string DatabaseFolderPath { get; set; } = null!;

    [Test]
    public void TheTest() {
      // Neither the configuration file nor the database folder exist.
      var exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for missing configuration file.");
      Assert.IsTrue(
        exception.Message.StartsWith("Please edit database configuration file '"),
        "Missing configuration file message");
      // The configuration file has been created and already contains
      // the database folder path we want to use.
      // So we don't actually need to edit the configuration file.  
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for missing database folder.");
      Assert.IsTrue(
        exception.Message.StartsWith("Database folder '"),
        "Missing database folder message");
      Directory.CreateDirectory(DatabaseFolderPath);
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for missing licence file.");
      Assert.IsTrue(
        exception.Message.StartsWith("VelocityDB licence file '"),
        "Missing licence file message");
      UpdateVelocityDbLicenceFilePath();
      Connection.Open();
      Assert.AreEqual(DatabaseFolderPath.ToLower(), Global.Session.SystemDirectory,
        "SystemDirectory");
      Assert.AreEqual(Connection.ExpectedVersion, Schema.Instance.Version, "Version");
      ResetSchemaVersionToZero();
      // As we have just made the schema version out of date,
      // the entity types have to be registered again.
      // However, a copy of the licence file is now already on the database,
      // so we don't have to copy it again.
      Connection.Open();
      Assert.AreEqual(Connection.ExpectedVersion, Schema.Instance.Version, "Version #2");
      RemoveXmlElement();
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for missing XML element.");
      Assert.IsTrue(
        exception.Message.Contains(
          " tag was not found in database configuration file "),
        "Missing XML element");
      MakeXmlError();
      exception = Assert.Catch<ApplicationException>(
        () => Connection.Open(),
        "Open should have thrown ApplicationException for XML error.");
      Assert.IsTrue(
        exception.Message.StartsWith(
          "The following XML error was found in database configuration file "),
        "XML error");
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

    private void ResetSchemaVersionToZero() {
      var session = new SessionNoServer(DatabaseFolderPath);
      session.BeginUpdate();
      var schema = Schema.Find(QueryHelper.Instance, session)!;
      Assert.IsNotNull(schema, "schema");
      schema.Version = 0;
      session.Commit();
    }

    private void UpdateVelocityDbLicenceFilePath() {
      string configText;
      using (var reader = new StreamReader(ConfigFilePath)) {
        configText = reader.ReadToEnd();
      }
      using (var writer = new StreamWriter(ConfigFilePath)) {
        writer.Write(configText.Replace("For developer use only",
          TestSession.VelocityDbLicenceFilePath));
      }
    }
  }
}