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
    // private string? LicenceFileCopyPath { get; set; }

    public void Open() {
      DatabaseConfig = CreateDatabaseConfig();
      DatabaseConfig.Load();
      CheckDatabaseFolderExists();
      CopySystemDatabaseFilesToDatabaseFolderIfRequired();
      Schema schema;
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      try {
        schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
        if (schema.Version < ExpectedVersion) {
          CopyLicenceToDatabaseFolderIfAbsent();
          schema.RegisterPersistableTypes(session);
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
// #if DEBUG
// #else // Release build
//       if (LicenceFileCopyPath != null) {
//         // A VelocityDB licence file was copied to the database so that the persistable
//         // types could be registered. Now that the registration has been done (and we are
//         // no longer in a transaction), the licence file can safely be removed from the
//         // database, provided no further additions or deletions of persistable types are
//         // to be made. The licence file should be removed for a database that is to be
//         // given to end users.
//         File.Delete(LicenceFileCopyPath);
//       }
// #endif
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
      // LicenceFileCopyPath = Path.Combine(
      string licenceFileCopyPath = Path.Combine(
        DatabaseConfig.DatabaseFolderPath, "4.odb");
      // string destinationPath = 
      //   $"{DatabaseConfig.DatabaseFolderPath}{Path.DirectorySeparatorChar}4.odb";
      if (File.Exists(licenceFileCopyPath)) {
        return;
      }
      CheckLicenceFileExists();
      CopyLicenceFile(
        DatabaseConfig.VelocityDbLicenceFilePath, 
        licenceFileCopyPath);
    }

    [ExcludeFromCodeCoverage]
    private void CopySystemDatabaseFilesToDatabaseFolderIfRequired() {
      bool isDatabaseFolderEmpty =
        !Directory.GetFiles(DatabaseConfig.DatabaseFolderPath).Any();
      if (isDatabaseFolderEmpty) {
        var systemDatabaseFilesFolder = new DirectoryInfo(
          Path.Combine(Global.GetApplicationFolderPath(), "System Database Files"));
        // var systemDatabaseFilesFolder = new DirectoryInfo(
        //   Global.GetApplicationFolderPath() +
        //   Path.DirectorySeparatorChar + "System Database Files");
        if (systemDatabaseFilesFolder.Exists) {
          foreach (FileInfo sourceFile in systemDatabaseFilesFolder.GetFiles()) {
            string destinationPath = sourceFile.FullName.Replace(
              systemDatabaseFilesFolder.FullName,
              DatabaseConfig.DatabaseFolderPath);
            sourceFile.CopyTo(destinationPath);
          }
        }
      }
    }
  }
}