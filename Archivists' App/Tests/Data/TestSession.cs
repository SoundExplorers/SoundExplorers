using System;
using System.IO;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  internal class TestSession : SessionNoServer {
    public const string DatabaseParentFolderPath = "C:\\Simon\\Databases";

    public const string VelocityDbLicenceFilePath =
      @"E:\Simon\OneDrive\Documents\My Installers\VelocityDB\License Database\4.odb";

    public TestSession() : base(CreateDatabaseFolder()) {
      BeginUpdate();
      new Schema().RegisterEntityTypes(this);
      Commit();
    }

    public TestSession(string databaseFolderPath) : base(databaseFolderPath) { }
    private string DatabaseFolderPath => SystemDirectory;

    public static void CopyLicenceToDatabaseFolder(string databaseFolderPath) {
      File.Copy(
        VelocityDbLicenceFilePath,
        databaseFolderPath + @"\4.odb");
    }

    public static string CreateDatabaseFolder() {
      string databaseFolderPath = GenerateDatabaseFolderPath();
      Directory.CreateDirectory(databaseFolderPath);
      CopyLicenceToDatabaseFolder(databaseFolderPath);
      return databaseFolderPath;
    }

    public void DeleteDatabaseFolderIfExists() {
      DeleteFolderIfExists(DatabaseFolderPath);
    }

    public static void DeleteFolderIfExists(string folderPath) {
      if (Directory.Exists(folderPath)) {
        foreach (string filePath in Directory.GetFiles(folderPath)) {
          File.Delete(filePath);
        }
        Directory.Delete(folderPath);
      }
    }

    private static string GenerateDatabaseFolderPath() {
      return DatabaseParentFolderPath + "\\Database" + DateTime.Now.Ticks;
    }
  }
}