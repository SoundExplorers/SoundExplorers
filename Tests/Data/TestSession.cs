using System;
using System.IO;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Data {
  public class TestSession : SessionNoServer {
    public const string DatabaseParentFolderPath = "C:\\Simon\\Databases";

    /// <summary>
    ///   TODO Replace hard coded VelocityDbLicenceFilePath value to support multiple developers.
    /// </summary>
    public const string VelocityDbLicenceFilePath =
      @"E:\Simon\OneDrive\Documents\My Installers\VelocityDB\License Database\4.odb";

    public TestSession() : base(CreateDatabaseFolder()) {
      BeginUpdate();
      new Schema().RegisterPersistableTypes(this);
      Commit();
    }

    public TestSession(string databaseFolderPath) : base(databaseFolderPath) { }
    internal string DatabaseFolderPath => SystemDirectory;

    public static void CopyLicenceToDatabaseFolder(string databaseFolderPath) {
      File.Copy(
        VelocityDbLicenceFilePath, 
        Path.Combine(databaseFolderPath, "4.odb"));
      // File.Copy(
      //   VelocityDbLicenceFilePath,
      //   databaseFolderPath + @"\4.odb");
    }

    public static string CreateDatabaseFolder() {
      string databaseFolderPath = GenerateDatabaseFolderPath();
      Directory.CreateDirectory(databaseFolderPath);
      CopyLicenceToDatabaseFolder(databaseFolderPath);
      return databaseFolderPath;
    }

    public static void DeleteFolderIfExists(string folderPath) {
      if (Directory.Exists(folderPath)) {
        foreach (string filePath in Directory.GetFiles(folderPath)) {
          File.Delete(filePath);
        }
        Directory.Delete(folderPath);
      }
    }

    public void DeleteDatabaseFolderIfExists() {
      DeleteFolderIfExists(DatabaseFolderPath);
    }

    private static string GenerateDatabaseFolderPath() {
      return DatabaseParentFolderPath + "\\Database" + DateTime.Now.Ticks;
    }
  }
}