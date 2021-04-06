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

    public void Open() {
      DatabaseConfig = CreateDatabaseConfig();
      DatabaseConfig.Load();
      CheckDatabaseFolderExists();
      // For a database that is used by end users, run
      // DatabaseGenerator.GenerateInitialisedDatabase to create the required initialised
      // system database files, which will be copied by the installer into the
      // application folder.
      CopySystemDatabaseFilesToDatabaseFolderIfRequired();
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

    [ExcludeFromCodeCoverage]
    private void CopySystemDatabaseFilesToDatabaseFolderIfRequired() {
      bool isDatabaseFolderEmpty =
        !Directory.GetFiles(DatabaseConfig.DatabaseFolderPath).Any();
      if (isDatabaseFolderEmpty) {
        var initialisedDatabaseFolder = new DirectoryInfo(
          Path.Combine(Global.GetApplicationFolderPath(), "Initialised Database"));
        if (initialisedDatabaseFolder.Exists) {
          foreach (FileInfo sourceFile in initialisedDatabaseFolder.GetFiles()) {
            string destinationPath = sourceFile.FullName.Replace(
              initialisedDatabaseFolder.FullName,
              DatabaseConfig.DatabaseFolderPath);
            sourceFile.CopyTo(destinationPath);
          }
        }
      }
    }
  }
}