using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Utilities {
  /// <summary>
  ///   A utility for generating an initialised database and/or a test database.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public class DatabaseGenerator {
    /// <summary>
    ///   Gets the path of the initialised database folder that is to be a source for the
    ///   installer.
    /// </summary>
    public string? InitialisedDatabaseFolderPath { get; private set; }

    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    public void GenerateTestDatabase(int eventCount, int startYear,
      bool keepLicenceFile) {
      Console.WriteLine(
        "Generating test database in folder " +
        $"'{DatabaseConfig.DefaultDatabaseFolderPath}'.");
      InitialiseDatabase(DatabaseConfig.DefaultDatabaseFolderPath, keepLicenceFile);
      Data = new TestData(new QueryHelper(), startYear);
      Session.BeginUpdate();
      Data.AddActsPersisted(Session);
      Data.AddArtistsPersisted(200, Session);
      Data.AddEventTypesPersisted(Session);
      Data.AddGenresPersisted(Session);
      Data.AddLocationsPersisted(Session);
      Data.AddNewslettersPersisted(eventCount, Session);
      Data.AddRolesPersisted(Session);
      Data.AddSeriesPersisted(Session);
      Data.AddUserOptionsPersisted(3, Session);
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
        $"Finished: {Data.Events.Count:#,0} Events; {Data.Sets.Count:#,0} Sets; " +
        $"{Data.Pieces.Count:#,0} Pieces; {Data.Credits.Count:#,0} Credits.\r\n" +
        $"First Newsletter Date: {Data.FirstNewsletterDate:ddd dd MMM yyyy}\r\n" +
        $"Last Event Date: {Data.LastEventDate:ddd dd MMM yyyy}");
    }

    /// <summary>
    ///   Generates an initialised database folder, suitable for first use by end users.
    /// </summary>
    /// <remarks>
    ///   The initialised database folder will including only the two system database
    ///   files that contain the schema objects and DatabaseLocation objects
    ///   respectively. The installer will include a copy of the initialised database
    ///   folder. DatabaseConnection.Open will copy these system database files to
    ///   the end user's database folder if it is found to be empty.
    /// </remarks>
    public void GenerateInitialisedDatabase() {
      InitialisedDatabaseFolderPath =
        Path.Combine(GetInstallerDataFolderPath(), "Initialised Database");
      InitialiseDatabase(InitialisedDatabaseFolderPath, false);
      Console.WriteLine(
        $"Generated initialised database folder '{InitialisedDatabaseFolderPath}'.");
    }

    /// <summary>
    ///   Gets the path of the Installer\Data subfolder of the solution folder, into
    ///   which an initialised database folder, suitable for first use by end users, will
    ///   be copied.
    /// </summary>
    private static string GetInstallerDataFolderPath() {
      string testBinFolderPath = Global.GetApplicationFolderPath();
      string solutionFolderPath = testBinFolderPath.Substring(0,
        testBinFolderPath.IndexOf(
          @"\Tests\", StringComparison.OrdinalIgnoreCase));
      string result = Path.Combine(solutionFolderPath, @"Installer\Data");
      Assert.IsTrue(Directory.Exists(result),
        $"Cannot find installer data folder '{result}'.");
      return result;
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

    private void InitialiseDatabase(string databaseFolderPath, bool keepLicenceFile) {
      TestSession.DeleteFolderIfExists(databaseFolderPath);
      Directory.CreateDirectory(databaseFolderPath);
      TestSession.CopyLicenceToDatabaseFolder(databaseFolderPath);
      Session = new TestSession(databaseFolderPath);
      Session.BeginUpdate();
      // Registering the persistable types will allow the database to be used without a
      // VelocityDB licence file.
      Schema.Instance.RegisterPersistableTypes(Session);
      Session.Commit();
      if (!keepLicenceFile) {
        RemoveLicenceFileFromDatabase();
      }
      // Remove unwanted database files, which should only be the transaction file
      // (0.odb).
      var databaseFolder = new DirectoryInfo(databaseFolderPath);
      foreach (var file in databaseFolder.GetFiles()) {
        if (string.Compare(
              file.Name, "1.odb", StringComparison.OrdinalIgnoreCase) != 0
            && string.Compare(
              file.Name, "2.odb", StringComparison.OrdinalIgnoreCase) != 0) {
          file.Delete();
        }
      }
    }

    /// <summary>
    ///   Once the persistable type registration
    ///   (<see cref="Schema.RegisterPersistableTypes" />) has been done (and we are no
    ///   longer in a transaction), the licence file can safely be removed from the
    ///   database, provided no further changes to persistable types are subsequently to be
    ///   made. The licence file should be removed from a database that is to be given to
    ///   end users.
    /// </summary>
    private void RemoveLicenceFileFromDatabase() {
      File.Delete(Path.Combine(Session.SystemDirectory, "4.odb"));
      Console.WriteLine("Removed licence file from database.");
    }
  }
}