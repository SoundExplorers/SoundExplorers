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

    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    /// <summary>
    ///   If the main test database folder already exists, it will be deleted and
    ///   recreated from scratch.
    /// </summary>
    [Test]
    public void GenerateData() {
      if (DoIt == 1) {
        Generate();
      }
    }

    private void Generate() {
      Data = new TestData(new QueryHelper());
      TestSession.DeleteFolderIfExists(DatabaseConfig.DefaultDatabaseFolderPath);
      Directory.CreateDirectory(DatabaseConfig.DefaultDatabaseFolderPath);
      TestSession.CopyLicenceToDatabaseFolder(DatabaseConfig.DefaultDatabaseFolderPath);
      Session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddActsPersisted(10, Session);
      Data.AddEventTypesPersisted(5, Session);
      Data.AddGenresPersisted(10, Session);
      Data.AddLocationsPersisted(8, Session);
      Data.AddNewslettersPersisted(64, Session);
      Data.AddSeriesPersisted(8, Session);
      AddEvents();
      AddSets();
      Session.Commit();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddEvents() {
      Data.AddEventsPersisted(18, Session);
      for (int i = 0; i < 5; i++) {
        Data.Events[i].EventType = Data.EventTypes[i];
      }
      for (int i = 5; i < 10; i++) {
        Data.Events[i].EventType = Data.EventTypes[i - 5];
      }
      for (int i = 10; i < 15; i++) {
        Data.Events[i].EventType = Data.EventTypes[i - 10];
      }
      for (int i = 0; i < 16; i++) {
        Data.Events[i].Newsletter = Data.Newsletters[i];
      }
      for (int i = 0; i < 8; i++) {
        Data.Events[i].Location = Data.Locations[i];
        Data.Events[i].Series = Data.Series[i];
      }
      for (int i = 8; i < 16; i++) {
        Data.Events[i].Location = Data.Locations[i - 8];
        Data.Events[i].Series = Data.Series[i - 8];
      }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddSets() {
      for (int i = 0; i < 5; i++) {
        Data.AddSetsPersisted(i + 1, Session, Data.Events[i], Data.Genres[i]);
      }
      for (int i = 5; i < 10; i++) {
        Data.AddSetsPersisted(i - 4, Session, Data.Events[i], Data.Genres[i]);
      }
      for (int i = 10; i < 15; i++) {
        Data.AddSetsPersisted(i - 9, Session, Data.Events[i], Data.Genres[i - 10]);
      }
      for (int i = 15; i < 18; i++) {
        Data.AddSetsPersisted(i - 9, Session, Data.Events[i]);
      }
      for (int i = 0; i < Data.Events.Count; i++) {
        var @event = Data.Events[i];
        bool isEventIndexEven = i % 2 == 0;
        for (int j = 0; j < @event.Sets.Count; j++) {
          @event.Sets[j].Act = isEventIndexEven ? Data.Acts[j] : Data.Acts[j + 2];
        }
      }
    }
  }
}