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

    public string DatabaseFolderPath { get; }

    private TestSession(string databaseFolderPath) : base(databaseFolderPath) {
      DatabaseFolderPath = databaseFolderPath;
    }

    [NotNull]
    public static TestSession Create() {
      var databaseFolderPath = GetTempDatabaseFolderPath();
      RemoveFolderIfExists(databaseFolderPath);
      Directory.CreateDirectory(databaseFolderPath);
      var session = new TestSession(databaseFolderPath);
      return session;
    }

    public new void Dispose() {
      base.Dispose();
      RemoveFolderIfExists(DatabaseFolderPath);
    }

    [NotNull]
    public OptimizedPersistable ReadUsingIndex(Func<OptimizedPersistable> readFunction) {
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

    private static string GetTempDatabaseFolderPath() {
      return DatabaseParentFolderPath + "\\Database" + DateTime.Now.Ticks;
    }

    private static void RemoveFolderIfExists(string folderPath) {
      if (!Directory.Exists(folderPath)) {
        return;
      }
      foreach (var filePath in Directory.GetFiles(folderPath)) {
        File.Delete(filePath);
      }
      Directory.Delete(folderPath);
    }
  }
}