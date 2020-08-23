using System;
using System.IO;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  internal class TestSession : SessionNoServer, IDisposable {
    private const string DatabaseParentFolderPath = "C:\\Simon\\Databases";
    public TestSession(string databaseFolderPath) : base(databaseFolderPath) { }

    private static void CopyLicenceToDatabaseFolder(string databaseFolderPath) {
      File.Copy(
        @"E:\Simon\OneDrive\Documents\My Installers\VelocityDB\License Database\4.odb",
        databaseFolderPath + @"\4.odb");
    }

    [NotNull]
    public static string CreateDatabaseFolder() {
      string databaseFolderPath = GenerateDatabaseFolderPath();
      Directory.CreateDirectory(databaseFolderPath);
      CopyLicenceToDatabaseFolder(databaseFolderPath);
      return databaseFolderPath;
    }

    public static void DeleteFolderIfExists([NotNull] string folderPath) {
      if (Directory.Exists(folderPath)) {
        foreach (string filePath in Directory.GetFiles(folderPath)) {
          File.Delete(filePath);
        }
      }
      Directory.Delete(folderPath);
    }

    private static string GenerateDatabaseFolderPath() {
      return DatabaseParentFolderPath + "\\Database" + DateTime.Now.Ticks;
    }
  }
}