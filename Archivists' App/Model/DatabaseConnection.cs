using System;
using System.IO;
using JetBrains.Annotations;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class DatabaseConnection {
    private const int ExpectedVersion = 1;

    public void Open() {
      var databaseConfig = new DatabaseConfig();
      databaseConfig.Load();
      CreateDatabaseFolderIfRequired(databaseConfig);
      var session = new SessionNoServer(databaseConfig.DatabaseFolderPath);
      session.BeginUpdate();
      var schema = Schema.Find(QueryHelper.Instance, session) ?? new Schema();
      if (schema.Version < ExpectedVersion) {
        schema.Upgrade(ExpectedVersion, session);
      }
      if (!schema.IsPersistent) {
        session.Persist(schema);
      }
      session.Commit();
      Global.Session = session;
      Schema.Instance = schema;
    }

    private static void CreateDatabaseFolderIfRequired(
      [NotNull] DatabaseConfig databaseConfig) {
      try {
        if (!Directory.Exists(databaseConfig.DatabaseFolderPath)) {
          Directory.CreateDirectory(databaseConfig.DatabaseFolderPath);
        }
      } catch (Exception exception) {
        throw Global.CreateFileException(exception,
          $"As specified in {databaseConfig.ConfigFilePath},"
          + $"{Environment.NewLine} database folder path",
          databaseConfig.DatabaseFolderPath);
      }
      CopyLicenceToDatabaseFolderIfRequired(databaseConfig.DatabaseFolderPath);
    }

    private static void CopyLicenceToDatabaseFolderIfRequired(
      [NotNull] string databaseFolderPath) {
      var sourcePath =
        $"{Global.GetApplicationFolderPath()}{Path.DirectorySeparatorChar}4.odb";
      var destinationPath = $"{databaseFolderPath}{Path.DirectorySeparatorChar}4.odb";
      try {
        File.Copy(sourcePath, destinationPath);
      } catch (Exception exception) {
        throw Global.CreateFileException(exception, "VelocityDB licence file",
          sourcePath);
      }
    }
  }
}