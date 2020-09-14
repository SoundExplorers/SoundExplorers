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
      CreateDatabaseFolderIfRequired();
      CopyLicenceToDatabaseFolderIfRequired();
      var session = new SessionNoServer(DatabaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      var schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
      if (schema.Version < ExpectedVersion) {
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

    private void CreateDatabaseFolderIfRequired() {
      try {
        if (!Directory.Exists(DatabaseConfig.DatabaseFolderPath)) {
          Directory.CreateDirectory(DatabaseConfig.DatabaseFolderPath);
        }
      } catch (Exception exception) {
        throw Global.CreateFileException(exception,
          $"As specified in {DatabaseConfig.ConfigFilePath},"
          + $"{Environment.NewLine} database folder path",
          DatabaseConfig.DatabaseFolderPath);
      }
    }

    private void CopyLicenceToDatabaseFolderIfRequired() {
      var destinationPath =
        $"{DatabaseConfig.DatabaseFolderPath}{Path.DirectorySeparatorChar}4.odb";
      if (File.Exists(destinationPath)) {
        return;
      }
      var sourcePath =
        $"{Global.GetApplicationFolderPath()}{Path.DirectorySeparatorChar}4.odb";
      try {
        File.Copy(sourcePath, destinationPath);
      } catch (Exception exception) {
        throw Global.CreateFileException(exception, "VelocityDB licence file",
          sourcePath);
      }
    }
  }
}