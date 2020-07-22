using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  internal class TestSession : SessionNoServer, IDisposable {
    private const string DatabaseParentFolderPath = "C:\\Simon";

    public TestSession(string databaseFolderPath) : base(databaseFolderPath) {
      DatabaseFolderPath = databaseFolderPath;
    }

    public TestSession() : this(CreateDatabaseFolder()) { }

    public string DatabaseFolderPath { get; }
    public bool OnDisposeDeleteDatabaseFolder { get; set; }

    public new void Dispose() {
      base.Dispose();
      if (OnDisposeDeleteDatabaseFolder) {
        DeleteFolderIfExists(DatabaseFolderPath);
      }
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

    [NotNull]
    public OptimizedPersistable ReadUsingIndex(
      Func<OptimizedPersistable> readFunction) {
      OptimizedPersistable result;
      using (var traceWriter = new StringWriter()) {
        using (var traceListener = new TextWriterTraceListener(traceWriter)) {
          Trace.Listeners.Add(traceListener);
          TraceIndexUsage = true;
          result = readFunction();
          TraceIndexUsage = false; // Seems not to work
          Trace.Listeners.Remove(traceListener);
        }
        if (!traceWriter.ToString().Contains("Index used")) {
          throw new DataException("An index was not used.");
        }
      }
      return result;
    }

    private static void CopyLicenceToDatabaseFolder(string databaseFolderPath) {
      File.Copy(
        @"E:\Simon\OneDrive\Documents\My Installers\VelocityDB\License Database\4.odb",
        databaseFolderPath + @"\4.odb");
    }

    private static string GenerateDatabaseFolderPath() {
      return DatabaseParentFolderPath + "\\Database" + DateTime.Now.Ticks;
    }
  }
}