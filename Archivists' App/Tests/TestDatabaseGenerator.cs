using System.Diagnostics.CodeAnalysis;
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
    private static int DoIt => 0;

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
      for (var i = 0; i < 5; i++) {
        data.AddSetsPersisted(i + 1, session, data.Events[i], data.Genres[i]);
      }
      for (var i = 5; i < 10; i++) {
        data.AddSetsPersisted(i - 4, session, data.Events[i], data.Genres[i]);
      }
      for (var i = 10; i < 15; i++) {
        data.AddSetsPersisted(i - 9, session, data.Events[i], data.Genres[i - 10]);
      }
      for (var i = 15; i < 18; i++) {
        data.AddSetsPersisted(i - 9, session, data.Events[i]);
      }
      for (var i = 0; i < 10; i++) {
        data.Sets[i].Act = data.Acts[i];
      }
      for (var i = 10; i < 15; i++) {
        data.Sets[i].Act = data.Acts[i - 10];
      }
      for (var i = 15; i < 20; i++) {
        data.Sets[i].Act = data.Acts[i - 15];
      }
      for (var i = 20; i < 25; i++) {
        data.Sets[i].Act = data.Acts[i - 20];
      }
      for (var i = 25; i < 30; i++) {
        data.Sets[i].Act = data.Acts[i - 25];
      }
      for (var i = 30; i < 35; i++) {
        data.Sets[i].Act = data.Acts[i - 30];
      }
      for (var i = 35; i < 40; i++) {
        data.Sets[i].Act = data.Acts[i - 35];
      }
      for (var i = 40; i < 45; i++) {
        data.Sets[i].Act = data.Acts[i - 40];
      }
      session.Commit();
    }
  }
}