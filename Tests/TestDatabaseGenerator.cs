using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests {
  /// <remarks>
  ///   The <see cref="ExplicitAttribute" /> instructs NUnit to ignored the generator
  ///   test(s) unless explicitly selected for running. We don't want to generate the
  ///   main test database, which is used for GUI tests, every time all tests are run.
  /// </remarks>
  [TestFixture] [Explicit]
  [ExcludeFromCodeCoverage]
  public class TestDatabaseGenerator {
    private static int EventCount => 112;
    private static int StartYear => 2019;

    /// <summary>
    ///   The path of the folder, usually the application project's release build folder,
    ///   into which an initialised database folder, suitable for first use by end users,
    ///   will be copied.  
    /// </summary>
    /// <remarks>
    ///   The initialised database folder will including only the two system database
    ///   files that contain the schema objects and DatabaseLocation objects
    ///   respectively, together with the database file containing the Schema
    ///   persistable. The installer will include a copy of the initialised database
    ///   folder. <see cref="DatabaseConnection.Open" /> will copy these system files to
    ///   the end user's database folder if it is found to be empty.
    /// </remarks>
    private static string InitialisedDatabaseParentFolderPath =>
      @"E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\x64\Release\net5.0-windows";
    
    private TestData Data { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    /// <summary>
    ///   If the main test database folder already exists, it will be deleted and
    ///   recreated from scratch.
    /// </summary>
    [Test]
    public void A01_GenerateData() {
      Data = new TestData(new QueryHelper(), StartYear);
      TestSession.DeleteFolderIfExists(DatabaseConfig.DefaultDatabaseFolderPath);
      Directory.CreateDirectory(DatabaseConfig.DefaultDatabaseFolderPath);
      TestSession.CopyLicenceToDatabaseFolder(DatabaseConfig.DefaultDatabaseFolderPath);
      Session = new TestSession(DatabaseConfig.DefaultDatabaseFolderPath);
      Session.BeginUpdate();
      // Registering the persistable types will allow the database to be used without a
      // VelocityDB licence file.
      Schema.Instance.RegisterPersistableTypes(Session);
      // But, for some reason to do with VelocityDB Indexes, we also need to add data of
      // every entity type here, which automatically adds an index of each type.
      // Otherwise the application will crash on loading. That is why we even add some
      // dummy UserOptions.
      //
      // The Schema persistable, with the current schema version number, is required for
      // copying to the initialised database folder.
      Data.AddSchemaPersisted(1, Session); 
      Data.AddActsPersisted(Session);
      Data.AddArtistsPersisted(200, Session);
      Data.AddEventTypesPersisted(Session);
      Data.AddGenresPersisted(Session);
      Data.AddLocationsPersisted(Session);
      Data.AddNewslettersPersisted(EventCount, Session);
      Data.AddRolesPersisted(Session);
      Data.AddSeriesPersisted(Session);
      Data.AddUserOptionsPersisted(3, Session);
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
#if DEBUG
#else // Release build
      // A VelocityDB licence file was copied to the database so that the persistable
      // types could be registered. Now that the registration has been done (and we are
      // no longer in a transaction), the licence file can safely be removed from the
      // database, provided no further additions or deletions of persistable types are
      // subsequently to be made. The licence file should be removed for a database that
      // is to be given to end users.
      RemoveLicenceFileFromDatabase();
#endif
      Console.WriteLine(
        $"Finished: {Data.Events.Count:#,0} Events; {Data.Sets.Count:#,0} Sets; " +
        $"{Data.Pieces.Count:#,0} Pieces; {Data.Credits.Count:#,0} Credits.\r\n" + 
        $"First Newsletter Date: {Data.FirstNewsletterDate:ddd dd MMM yyyy}\r\n" +
        $"Last Event Date: {Data.LastEventDate:ddd dd MMM yyyy}");
      CreateInitialisedDatabaseFolder();
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

    private void CreateInitialisedDatabaseFolder() {
      var initialisedDatabaseParentFolder =
        new DirectoryInfo(InitialisedDatabaseParentFolderPath);
      Assert.IsTrue(initialisedDatabaseParentFolder.Exists, 
        $"Cannot find initialised database parent folder '{InitialisedDatabaseParentFolderPath}'.");
      var initialisedDatabaseFolder = new DirectoryInfo(
        Path.Combine(InitialisedDatabaseParentFolderPath, "Initialised Database"));
      if (initialisedDatabaseFolder.Exists) {
        initialisedDatabaseFolder.Delete(true);
      }
      initialisedDatabaseFolder.Create();
      var sourceSystemFile1 = new FileInfo(
        Path.Combine(Session.DatabaseFolderPath, "1.odb"));
      var sourceSystemFile2 = new FileInfo(
        Path.Combine(Session.DatabaseFolderPath, "2.odb"));
      var destinationSystemFile1 = new FileInfo(
        Path.Combine(initialisedDatabaseFolder.FullName, "1.odb"));
      var destinationSystemFile2 = new FileInfo(
        Path.Combine(initialisedDatabaseFolder.FullName, "2.odb"));
      sourceSystemFile1.CopyTo(destinationSystemFile1.FullName);
      sourceSystemFile2.CopyTo(destinationSystemFile2.FullName);
      // We need to include the Schema persistable too. Otherwise, if there is no
      // licence file, DatabaseConnection.Open will want to copy one into the database
      // folder while the application is loading.
      string schemaDatabaseFileName =
        Session.DatabaseNumberOf(typeof(Schema)) + ".odb";
      var sourceSchemaDatabaseFile = new FileInfo(
        Path.Combine(Session.DatabaseFolderPath, schemaDatabaseFileName));
      var destinationSchemaDatabaseFile = new FileInfo(
        Path.Combine(initialisedDatabaseFolder.FullName, schemaDatabaseFileName));
      sourceSchemaDatabaseFile.CopyTo(destinationSchemaDatabaseFile.FullName);
      Console.WriteLine(
        $"Created initialised database folder '{initialisedDatabaseFolder.FullName}'.");
    }

#if DEBUG
#else // Release build
    private void RemoveLicenceFileFromDatabase() {
      File.Delete(Path.Combine(Session.DatabaseFolderPath, "4.odb"));
      Console.WriteLine("Removed licence file from database.");
    }
#endif
  }
}