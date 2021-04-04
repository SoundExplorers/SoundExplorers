using System;
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
    private static int EventCount => 221;
    private static int StartYear => 2017;
    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    /// <summary>
    ///   If the main test database folder already exists, it will be deleted and
    ///   recreated from scratch.
    /// </summary>
    /// <remarks>
    ///   <see cref="ExplicitAttribute" /> instructs NUnit to ignored this test unless
    ///   it is explicitly selected for running. We don't want to generate the main test
    ///   database, which is used for GUI tests, every time all tests are run.
    /// </remarks>
    [Test] [Explicit]
    public void A01_GenerateData() {
      Data = new TestData(new QueryHelper(), StartYear);
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
      AddCredits();
      Console.WriteLine($"{Data.Credits.Count} Credits added. Committing.");
      Session.Commit();
      Console.WriteLine(
        $"Finished: {Data.Events.Count} Events; {Data.Sets.Count} Sets; " +
        $"{Data.Pieces.Count} Pieces; {Data.Credits.Count} Credits.\r\n" + 
        $"First Newsletter Date: {Data.FirstNewsletterDate:ddd dd MMM yyyy}\r\n" +
        $"Last Event Date: {Data.LastEventDate:ddd dd MMM yyyy}");
    }

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