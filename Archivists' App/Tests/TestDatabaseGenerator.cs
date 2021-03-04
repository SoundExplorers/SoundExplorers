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

    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    /// <summary>
    ///   If the main test database folder already exists, it will be deleted and
    ///   recreated from scratch.
    /// </summary>
    [Test]
    public void A01_GenerateData() {
      if (DoIt == 1) {
        Generate(20);
      }
    }

    [Test]
    public void A02_Fetch() {
      if (DoIt != 1) {
        return;
      }
      Session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      Session.BeginRead();
      var newsletters = Session.AllObjects<Newsletter>().ToList();
      Session.Commit();
      Assert.AreEqual(50, newsletters.Count);
    }

    private void Generate(int eventCount) {
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
      Console.WriteLine($"Adding {eventCount} Events.");
      Data.AddEventsPersisted(eventCount, Session);
      Console.WriteLine($"{eventCount} Events added. Adding Sets.");
      AddSets();
      Console.WriteLine($"{Data.Sets.Count} Sets added. Adding Pieces.");
      AddPieces();
      Console.WriteLine($"{Data.Pieces.Count} Pieces added. Adding Credits.");
      AddCredits();
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

    /// <summary>
    ///   Not currently included in the data generator, here are the other ways I tried
    ///   to vary adding the Credits for 50 Events without leading to a subsequent
    ///   SessionBase.AllObjects for any entity type throwing a StackOverFlowException.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void AddCreditsMultipleWays() {
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   var piece = @event.Sets[0].Pieces[0];
      //   int creditCount = TestData.GetRandomInteger(1, 4);
      //   for (int i = 0; i < creditCount; i++) {
      //     Data.AddCreditsPersisted(1, Session, piece);
      //   }
      //   if (@event.Sets.Count >= 2) {
      //     piece = @event.Sets[1].Pieces[0];
      //     creditCount = TestData.GetRandomInteger(1, 4);
      //     for (int i = 0; i < creditCount; i++) {
      //       Data.AddCreditsPersisted(1, Session, piece);
      //     }
      //   }
      // }
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   var set = @event.Sets[0];
      //   foreach (var piece in set.Pieces.Values) {
      //     int creditCount = TestData.GetRandomInteger(1, 4);
      //     for (int i = 0; i < creditCount; i++) {
      //       Data.AddCreditsPersisted(1, Session, piece);
      //     }
      //   }
      // }
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // foreach (var set in Data.Sets) {
      //   var piece = set.Pieces[0];
      //   int creditCount = TestData.GetRandomInteger(1, 4);
      //   for (int i = 0; i < creditCount; i++) {
      //     Data.AddCreditsPersisted(1, Session, piece);
      //   }
      // }
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   foreach (var set in @event.Sets.Values) {
      //     var piece = set.Pieces[0];
      //     int creditCount = TestData.GetRandomInteger(1, 4);
      //     for (int i = 0; i < creditCount; i++) {
      //       Data.AddCreditsPersisted(1, Session, piece);
      //     }
      //   }
      // }
      // Session.Commit();
      //
      // BIGGEST ONE THAT WORKS SO FAR WITH 50 EVENTS:
      //
      Session.BeginUpdate();
      foreach (var @event in Data.Events) {
        var piece = @event.Sets[0].Pieces[0];
        int creditCount = TestData.GetRandomInteger(1, 4);
        for (int i = 0; i < creditCount; i++) {
          Data.AddCreditsPersisted(1, Session, piece);
        }
      }
      Session.Commit();
      //
      // WORKS:
      //
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   Data.AddCreditsPersisted(1, Session, @event.Sets[0].Pieces[0]);
      // }
      // Session.Commit();
      //
      // SAME LOGIC AS AddPieces:
      //
      // Session.BeginUpdate();
      // foreach (var piece in Data.Pieces) {
      //   int creditCount = TestData.GetRandomInteger(1, 4);
      //   for (int i = 0; i < creditCount; i++) {
      //     Data.AddCreditsPersisted(1, Session, piece);
      //   }
      // }
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // foreach (var piece in Data.Pieces) {
      //   Data.AddCreditsPersisted(1, Session, piece);
      // }
      // Session.Commit();
      //
      // foreach (var @event in Data.Events) {
      //   foreach (var set in @event.Sets.Values) {
      //     foreach (var piece in set.Pieces.Values) {
      //       int creditCount = TestData.GetRandomInteger(1, 4);
      //       for (int i = 0; i < creditCount; i++) {
      //         Session.BeginUpdate();
      //         var piece1 = QueryHelper.Read<Piece>(piece.SimpleKey, set, Session);
      //         var artist = Data.GetRandomArtist();
      //         var artist1 = QueryHelper.Read<Artist>(artist.Name, Session);
      //         var role = Data.GetRandomRole();
      //         var role1 = QueryHelper.Read<Role>(role.Name, Session);
      //         Data.AddCreditsPersisted(1, Session, piece1, artist1, role1);
      //         Session.Commit();
      //       }
      //     }
      //   }
      // }
      //
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   var event1 = QueryHelper.Read<Event>(@event.SimpleKey, @event.Location, Session);
      //   foreach (var set in event1.Sets.Values) {
      //     var set1 = QueryHelper.Read<Set>(set.SimpleKey, event1, Session);
      //     foreach (var piece in set1.Pieces.Values) {
      //       var piece1 = QueryHelper.Read<Piece>(piece.SimpleKey, set, Session);
      //       var artist = Data.GetRandomArtist();
      //       var artist1 = QueryHelper.Read<Artist>(artist.Name, Session);
      //       var role = Data.GetRandomRole();
      //       var role1 = QueryHelper.Read<Role>(role.Name, Session);
      //       int creditCount = TestData.GetRandomInteger(1, 4);
      //       for (int i = 0; i < creditCount; i++) {
      //         Data.AddCreditsPersisted(1, Session, piece1, artist1, role1);
      //       }
      //     }
      //   }
      // }
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // foreach (var @event in Data.Events) {
      //   foreach (var set in @event.Sets.Values) {
      //     foreach (var piece in set.Pieces.Values) {
      //       var piece1 = QueryHelper.Read<Piece>(piece.SimpleKey, set, Session);
      //       var artist = Data.GetRandomArtist();
      //       var artist1 = QueryHelper.Read<Artist>(artist.Name, Session);
      //       var role = Data.GetRandomRole();
      //       var role1 = QueryHelper.Read<Role>(role.Name, Session);
      //       int creditCount = TestData.GetRandomInteger(1, 4);
      //       for (int i = 0; i < creditCount; i++) {
      //         Data.AddCreditsPersisted(1, Session, piece1, artist1, role1);
      //       }
      //     }
      //   }
      // }
      // Session.Commit();
      //
      // foreach (var @event in Data.Events) {
      //   foreach (var set in @event.Sets.Values) {
      //     foreach (var piece in set.Pieces.Values) {
      //       int creditCount = TestData.GetRandomInteger(1, 4);
      //       for (int i = 0; i < creditCount; i++) {
      //         Session.BeginUpdate();
      //         Data.AddCreditsPersisted(1, Session, piece);
      //         Session.Commit();
      //       }
      //     }
      //   }
      // }
      //
      // WORKS:
      // 
      // var event1 = Data.Events[0];
      // var event2 = Data.Events[1];
      // var artist1 = Data.Artists[0];
      // var artist2 = Data.Artists[1];
      // var artist3 = Data.Artists[2];
      // var role1 = Data.Roles[0];
      // var role2 = Data.Roles[1];
      // var role3 = Data.Roles[2];
      // Session.BeginUpdate();
      // var set1 = event1.Sets[0]; 
      // var piece1 = set1.Pieces[0];
      // Data.AddCreditsPersisted(1, Session, piece1, artist1, role1);
      // Data.AddCreditsPersisted(1, Session, piece1, artist2, role2);
      // var set2 = event2.Sets[0]; 
      // var piece2 = set2.Pieces[0];
      // Data.AddCreditsPersisted(1, Session, piece2, artist3, role3);
      // Data.AddCreditsPersisted(1, Session, piece2, artist2, role1);
      // Session.Commit();
      //
      // Session.BeginUpdate();
      // var pieces = Session.AllObjects<Piece>();
      // foreach (var piece in pieces) {
      //   int creditCount = TestData.GetRandomInteger(1, 4);
      //   for (int i = 0; i < creditCount; i++) {
      //     Data.AddCreditsPersisted(1, Session, piece);
      //   }
      // }
      // Session.Commit();
      //
      // foreach (var piece in Data.Pieces) {
      //   int creditCount = TestData.GetRandomInteger(1, 4);
      //   Session.BeginUpdate();
      //   var piece1 = QueryHelper.Read<Piece>(
      //     piece.SimpleKey, piece.IdentifyingParent, Session);
      //   for (int i = 0; i < creditCount; i++) {
      //     Data.AddCreditsPersisted(1, Session, piece1);
      //   }
      //   Session.Commit();
      // }
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