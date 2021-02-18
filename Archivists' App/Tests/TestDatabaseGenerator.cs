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
      Data.AddActsPersisted(Session);
      Data.AddEventTypesPersisted(Session);
      Data.AddGenresPersisted(Session);
      Data.AddLocationsPersisted(Session);
      Data.AddNewslettersPersisted(64, Session);
      Data.AddRolesPersisted(Session);
      Data.AddSeriesPersisted(Session);
      AddEvents();
      AddSets();
      Session.Commit();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddEvents() {
      Data.AddEventsPersisted(18, Session);
      for (int i = 0; i < Data.Events.Count; i++) {
        var @event = Data.Events[i];
        @event.EventType = Data.GetRandomEventType();
        @event.Location = Data.GetRandomLocation();
        if (i < Data.Events.Count - 2) {
          @event.Newsletter = Data.Newsletters[i];
          @event.Series = Data.GetRandomSeries();
        }
      }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddSets() {
      for (int i = 0; i < 5; i++) {
        Data.AddSetsPersisted(i + 1, Session, Data.Events[i]);
      }
      for (int i = 5; i < 10; i++) {
        Data.AddSetsPersisted(i - 4, Session, Data.Events[i]);
      }
      for (int i = 10; i < Data.Events.Count; i++) {
        Data.AddSetsPersisted(i - 9, Session, Data.Events[i]);
      }
      foreach (var set in Data.Sets) {
        set.Genre = Data.GetRandomGenre();
      }
      for (int i = 0; i < Data.Sets.Count; i++) {
        var set = Data.Sets[i];
        set.Genre = Data.GetRandomGenre();
        if (i < Data.Sets.Count - 2) {
          set.Act = Data.GetRandomAct();
        }
      }
    }
  }
}