using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class DatabaseConnection : IOpen {
    private DatabaseConfig DatabaseConfig { get; set; } = null!;
    public int ExpectedVersion { get; protected init; } = 1;

    [ExcludeFromCodeCoverage]
    public static void InitialiseDatabase(
      string sourceFolderPath, string destinationFolderPath) {
      CopySystemDatabaseFilesToDatabaseFolder(sourceFolderPath, destinationFolderPath);
      var session = new SessionNoServer(destinationFolderPath);
      session.RelocateDefaultDatabaseLocation();
      session.BeginUpdate();
      // Because we copied across the schema database file from the initialised
      // database, we have to explicitly change its location.
      session.RelocateDatabaseLocationFor(
        1, SessionBase.LocalHost, destinationFolderPath);
      session.Commit();
    }

    public void Open() {
      DatabaseConfig = CreateDatabaseConfig();
      DatabaseConfig.Load();
      CheckDatabaseFolderExists();
      InitialiseDatabase();
      Schema schema;
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      try {
        // foreach (var databaseLocation in session.DatabaseLocations) {
        //   Debug.WriteLine(databaseLocation.DirectoryPath);
        // }
        schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
        if (schema.Version < ExpectedVersion) {
          // In the release build, assume that the schema system database file that was
          // copied to the empty database folder already contains the persistable type
          // registrations that will allow the database to be accessed without a licence
          // file.
#if DEBUG
          CopyLicenceToDatabaseFolderIfAbsent();
          schema.RegisterPersistableTypes(session);
#endif
          schema.Version = ExpectedVersion;
        }
        if (!schema.IsPersistent) {
          session.Persist(schema);
        }
      } finally {
        session.Commit();
      }
      Global.Session = session;
      Schema.Instance = schema;
    }

    [ExcludeFromCodeCoverage]
    protected virtual DatabaseConfig CreateDatabaseConfig() {
      return new DatabaseConfig();
    }

    [ExcludeFromCodeCoverage]
    private static void CopySystemDatabaseFilesToDatabaseFolder(
      string sourceFolderPath, string destinationFolderPath) {
      var sourceFolder = new DirectoryInfo(sourceFolderPath);
      foreach (FileInfo sourceFile in sourceFolder.GetFiles()) {
        string destinationPath = sourceFile.FullName.Replace(
          sourceFolder.FullName,
          destinationFolderPath);
        sourceFile.CopyTo(destinationPath);
      }
    }

    private void CheckDatabaseFolderExists() {
      CheckDatabaseFolderPathHasBeenSpecified();
      if (!Directory.Exists(DatabaseConfig.DatabaseFolderPath)) {
        throw new ApplicationException(
          $"Database folder '{DatabaseConfig.DatabaseFolderPath}' cannot be found."
          + $"{Environment.NewLine}{Environment.NewLine}"
          + "Please specify the folder's path in database configuration file "
          + $"'{DatabaseConfig.ConfigFilePath}'.");
      }
    }

    private void CheckDatabaseFolderPathHasBeenSpecified() {
      if (!DatabaseConfig.HasDatabaseFolderPathBeenSpecified) {
        throw DatabaseConfig.CreateDatabaseFolderNotSpecifiedException();
      }
    }

#if DEBUG
    private void CheckLicenceFileExists() {
      if (!DatabaseConfig.HasLicenceFilePathBeenSpecified) {
        throw new ApplicationException(
          "Please specify the path of the VelocityDB licence file in "
          + $"database configuration file '{DatabaseConfig.ConfigFilePath}'.");
      }
      if (!File.Exists(DatabaseConfig.VelocityDbLicenceFilePath)) {
        throw new ApplicationException(
          $"VelocityDB licence file '{DatabaseConfig.VelocityDbLicenceFilePath}' "
          + "cannot be found."
          + $"{Environment.NewLine}{Environment.NewLine}"
          + "Please specify the file's path in database configuration file "
          + $"'{DatabaseConfig.ConfigFilePath}'.");
      }
    }

    [ExcludeFromCodeCoverage]
    private static void CopyLicenceFile(string sourcePath, string destinationPath) {
      try {
        File.Copy(sourcePath, destinationPath);
      } catch (Exception exception) {
        throw Global.CreateFileException(exception, "VelocityDB licence file",
          sourcePath);
      }
    }

    private void CopyLicenceToDatabaseFolderIfAbsent() {
      string licenceFileCopyPath = Path.Combine(
        DatabaseConfig.DatabaseFolderPath, "4.odb");
      if (File.Exists(licenceFileCopyPath)) {
        return;
      }
      CheckLicenceFileExists();
      CopyLicenceFile(
        DatabaseConfig.VelocityDbLicenceFilePath,
        licenceFileCopyPath);
    }
#endif

    /// <summary>
    ///   If the database folder is empty and the required initialised system database
    ///   files are available to copy into it, initialises the database with a predefined
    ///   schema that will allow the database to be used without a VelocityDB licence
    ///   file. This needs to be done if the database is to be updated by end users. If
    ///   the database folder is not empty, ensures that the database locations system
    ///   database file knows that the all the database files are in the current database
    ///   folder, in case this is a copy of a database created in another folder, which
    ///   may have been on another computer.
    /// </summary>
    /// <remarks>
    ///   To create the initialised system database files, first run
    ///   UtilityRunners.GenerateInitialisedDatabase, then rebuild the installer, which
    ///   will copy the files into a 'Initialised Database' subfolder of the application
    /// folder.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    private void InitialiseDatabase() {
      bool isDatabaseFolderEmpty =
        !Directory.GetFiles(DatabaseConfig.DatabaseFolderPath).Any();
      if (isDatabaseFolderEmpty) {
        string initialisedDatabaseFolderPath = 
          Path.Combine(Global.GetApplicationFolderPath(), "Initialised Database");
        if (Directory.Exists(initialisedDatabaseFolderPath)) {
          InitialiseDatabase(initialisedDatabaseFolderPath,
            DatabaseConfig.DatabaseFolderPath);
        }
      } else { // The database folder already contains files
        LocaliseDatabase();
      }
    }

    [ExcludeFromCodeCoverage]
    private void LocaliseDatabase() {
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.RelocateDefaultDatabaseLocation();
      session.BeginUpdate();
      foreach (var database in session.Databases) {
        session.RelocateDatabaseLocationFor(
          database.DatabaseNumber, SessionBase.LocalHost, 
          DatabaseConfig.DatabaseFolderPath);
      }
      session.Commit();
    }
  }
}