using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

    private static int EventCount => 40;
    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    /// <summary>
    ///   If the main test database folder already exists, it will be deleted and
    ///   recreated from scratch.
    /// </summary>
    [Test]
    public void A01_GenerateData() {
      if (DoIt == 1) {
        Generate();
      }
    }

    [Test]
    public void A02_Fetch() {
      if (DoIt != 1) {
        return;
      }
      Session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      Session.BeginRead();
      var acts = Session.AllObjects<Act>().ToList();
      var newsletters = Session.AllObjects<Newsletter>().ToList();
      Session.Commit();
      Assert.AreEqual(Data.Acts.Count, acts.Count);
      Assert.AreEqual(EventCount, newsletters.Count);
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
      Data.AddNewslettersPersisted(EventCount, Session);
      Data.AddRolesPersisted(Session);
      Data.AddSeriesPersisted(Session);
      Console.WriteLine($"Adding {EventCount} Events.");
      Data.AddEventsPersisted(EventCount, Session);
      Console.WriteLine($"{EventCount} Events added. Adding Sets.");
      AddSets();
      Console.WriteLine($"{Data.Sets.Count} Sets added. Adding Pieces.");
      AddPieces();
      Console.WriteLine($"{Data.Pieces.Count} Pieces added. Adding Credits.");
      // AddCredits();
      Console.WriteLine($"{Data.Credits.Count} Credits added. Committing.");
      Session.Commit();
      Console.WriteLine(
        $"Finished: {Data.Events.Count} Events; {Data.Sets.Count} Sets; " +
        $"{Data.Pieces.Count} Pieces; {Data.Credits.Count} Credits");
    }

    /// <summary>
    ///   With 10 Events, this works consistently.
    ///   With more Events, subsequent SessionBase.AllObjects for any entity type loops,
    ///   throwing a StackOverFlowException. See test A02_Fetch.
    ///   11 and 12 Events are borderline, depending on the quantity of Sets, Pieces and
    ///   Credits generated in each run.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddCredits() {
      foreach (var piece in Data.Pieces) {
        int creditCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < creditCount; i++) {
          Data.AddCreditsPersisted(1, Session, piece);
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
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddSets() {
      foreach (var @event in Data.Events) {
        int setCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < setCount; i++) {
          Data.AddSetsPersisted(1, Session, @event);
        }
      }
    }
  }
}