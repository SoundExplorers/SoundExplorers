using System;
using System.IO;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  internal class TestSession : SessionNoServer {
    //private static readonly string _defaultDatabaseFolderPath = "C:\\Simon\\Database";

    public string DatabaseFolderPath { get; }

    private TestSession(string databaseFolderPath) : base(databaseFolderPath) {
      DatabaseFolderPath = databaseFolderPath;
    }

    public static TestSession Create() {
      string databaseFolderPath = GetTempDatabaseFolderPath();
      RemoveFolderIfExists(databaseFolderPath);
      Directory.CreateDirectory(databaseFolderPath);
      var session = new TestSession(databaseFolderPath);
      return session;
    }

    public new void Dispose() {
      base.Dispose();
      RemoveFolderIfExists(DatabaseFolderPath);
    }

    private static string GetTempDatabaseFolderPath() {
      return "C:\\Simon\\Database" + DateTime.Now.Ticks;
    }

    private static void RemoveFolderIfExists(string folderPath) {
      if (Directory.Exists(folderPath)) {
        foreach (string filePath in Directory.GetFiles(folderPath)) {
          File.Delete(filePath);
        }
        Directory.Delete(folderPath);
      }
    }

  }
}