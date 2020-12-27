﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests {
  [TestFixture]
  [ExcludeFromCodeCoverage]
  public class TestDatabaseGenerator {
    /// <summary>
    ///   1 to enable generate
    /// </summary>
    private static int DoIt => 1;

    /// <summary>
    ///   If the main test database folder already exists,
    ///   it will be deleted and recreated from scratch.
    /// </summary>
    [Test]
    public void GenerateData() {
      if (DoIt == 1) {
        Generate();
      }
    }

    private static void Generate() {
      var queryHelper = new QueryHelper();
      var data = new TestData(queryHelper);
      TestSession.DeleteFolderIfExists(DatabaseConfig.DefaultDatabaseFolderPath);
      Directory.CreateDirectory(DatabaseConfig.DefaultDatabaseFolderPath);
      TestSession.CopyLicenceToDatabaseFolder(DatabaseConfig.DefaultDatabaseFolderPath);
      var session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      session.BeginUpdate();
      data.AddActsPersisted(10, session);
      data.AddEventTypesPersisted(5, session);
      data.AddGenresPersisted(10, session);
      data.AddLocationsPersisted(8, session);
      data.AddNewslettersPersisted(64, session);
      data.AddSeriesPersisted(8, session);
      data.AddEventsPersisted(18, session);
      for (var i = 0; i < 5; i++) {
        data.Events[i].EventType = data.EventTypes[i];
      }
      for (var i = 5; i < 10; i++) {
        data.Events[i].EventType = data.EventTypes[i - 5];
      }
      for (var i = 10; i < 15; i++) {
        data.Events[i].EventType = data.EventTypes[i - 10];
      }
      for (var i = 0; i < 16; i++) {
        data.Events[i].Newsletter = data.Newsletters[i];
      }
      for (var i = 0; i < 8; i++) {
        data.Events[i].Location = data.Locations[i];
        data.Events[i].Series = data.Series[i];
      }
      for (var i = 8; i < 16; i++) {
        data.Events[i].Location = data.Locations[i - 8];
        data.Events[i].Series = data.Series[i - 8];
      }
      data.AddSetsPersisted(24, session);
      session.Commit();
    }
  }
}