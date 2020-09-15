using System;
using System.IO;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class DatabaseConnection {
    private const int ExpectedVersion = 1;
    private DatabaseConfig DatabaseConfig { get; set; }

    public void Open() {
      DatabaseConfig = new DatabaseConfig();
      DatabaseConfig.Load();
      CheckDatabaseFolderExists();
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      var schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
      if (schema.Version < ExpectedVersion) {
        CopyLicenceToDatabaseFolderIfAbsent();
        schema.RegisterEntityTypes(session);
        schema.Version = ExpectedVersion;
      }
      if (!schema.IsPersistent) {
        session.Persist(schema);
      }
      session.Commit();
      Global.Session = session;
      Schema.Instance = schema;
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
          +"cannot be found."
          + $"{Environment.NewLine}{Environment.NewLine}"
          + "Please specify the file's path in database configuration file "
          + $"'{DatabaseConfig.ConfigFilePath}'.");
      }
    }

    private void CopyLicenceToDatabaseFolderIfAbsent() {
      var destinationPath =
        $"{DatabaseConfig.DatabaseFolderPath}{Path.DirectorySeparatorChar}4.odb";
      if (File.Exists(destinationPath)) {
        return;
      }
      CheckLicenceFileExists();
      string sourcePath = DatabaseConfig.VelocityDbLicenceFilePath;
      try {
        File.Copy(sourcePath, destinationPath);
      } catch (Exception exception) {
        throw Global.CreateFileException(exception, "VelocityDB licence file",
          sourcePath);
      }
    }
  }
}