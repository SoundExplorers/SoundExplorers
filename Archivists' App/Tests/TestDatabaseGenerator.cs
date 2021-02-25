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
      Data.AddArtistsPersisted(200, Session);
      Data.AddEventTypesPersisted(Session);
      Data.AddGenresPersisted(Session);
      Data.AddLocationsPersisted(Session);
      Data.AddNewslettersPersisted(50, Session);
      Data.AddRolesPersisted(Session);
      Data.AddSeriesPersisted(Session);
      AddEvents();
      AddSets();
      AddPieces();
      Session.Commit();
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddCredits() {
      foreach (var piece in Data.Pieces) {
        int creditCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < creditCount; i++) {
          Data.AddCreditsPersisted(1, Session, piece, Data.GetRandomArtist(),
            Data.GetRandomRole());
        }
      }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddEvents() {
      for (int i = 0; i < 50; i++) {
        Data.AddEventsPersisted(1, Session, Data.GetRandomLocation(),
          Data.GetRandomEventType());
        if (i < 48) {
          var @event = Data.Events[i];
          @event.Newsletter = Data.Newsletters[i + 1];
          @event.Series = Data.GetRandomSeries();
        }
      }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddPieces() {
      foreach (var set in Data.Sets) {
        int pieceCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < pieceCount; i++) {
          Data.AddPiecesPersisted(1, Session, set);
        }
      }
      for (int i = 0; i < Data.Pieces.Count; i++) {
        var piece = Data.Pieces[i];
        int chance = TestData.GetRandomInteger(1, 5);
        if (chance == 1) {
          piece.AudioUrl = string.Empty;
        }
        chance = TestData.GetRandomInteger(1, 5);
        if (chance > 1) {
          piece.VideoUrl = string.Empty;
        }
      }
      // Does not work to stick the blank cells at the end.
      // for (int i = Data.Pieces.Count - 2; i < Data.Pieces.Count; i++) {
      //   var piece = Data.Pieces[i];
      //   piece.AudioUrl = string.Empty;
      //   piece.VideoUrl = string.Empty;
      // }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddSets() {
      foreach (var @event in Data.Events) {
        int setCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < setCount; i++) {
          Data.AddSetsPersisted(1, Session, @event, Data.GetRandomGenre());
        }
      }
      for (int i = 0; i < Data.Sets.Count - 2; i++) {
        var set = Data.Sets[i];
        int chance = TestData.GetRandomInteger(1, 3);
        if (chance > 1) {
          set.Act = Data.GetRandomAct();
        }
      }
    }
  }
}