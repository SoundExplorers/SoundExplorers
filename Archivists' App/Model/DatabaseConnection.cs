using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SoundExplorers.Data;
using VelocityDb.Session;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace SoundExplorers.Model {
  public class DatabaseConnection : IOpen {
    private DatabaseConfig DatabaseConfig { get; set; }
    public int ExpectedVersion { get; protected set; } = 1;

    public void Open() {
      DatabaseConfig = CreateDatabaseConfig();
      DatabaseConfig.Load();
      CheckDatabaseFolderExists();
      Schema schema;
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      try {
        schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
        if (schema.Version < ExpectedVersion) {
          CopyLicenceToDatabaseFolderIfAbsent();
          schema.RegisterEntityTypes(session);
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
      if (!Directory.Exists(DatabaseConfig.DatabaseFolderPath)) {
        throw new ApplicationException(
          $"Database folder '{DatabaseConfig.DatabaseFolderPath}' cannot be found."
          + $"{Environment.NewLine}{Environment.NewLine}"
          + "Please specify the folder's path in database configuration file "
          + $"'{DatabaseConfig.ConfigFilePath}'.");
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
    private static void CopyLicenceFile([NotNull] string sourcePath,
      [NotNull] string destinationPath) {
      try {
        File.Copy(sourcePath, destinationPath);
      } catch (Exception exception) {
        throw Global.CreateFileException(exception, "VelocityDB licence file",
          sourcePath);
      }
    }

    private void CopyLicenceToDatabaseFolderIfAbsent() {
      string destinationPath =
        $"{DatabaseConfig.DatabaseFolderPath}{Path.DirectorySeparatorChar}4.odb";
      if (File.Exists(destinationPath)) {
        return;
      }
      CheckLicenceFileExists();
      CopyLicenceFile(DatabaseConfig.VelocityDbLicenceFilePath, destinationPath);
    }
  }
}