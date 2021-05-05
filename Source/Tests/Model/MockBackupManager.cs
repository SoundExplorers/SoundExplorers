using System;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockBackupManager : IBackupManager {
    internal string ErrorMessage { get; set; } = null!;
    public string BackupFolderPath { get; internal set; } = null!;
    public DateTime LastBackupDateTime { get; internal set; }

    public void BackupDatabaseTo(string backupFolderPath) {
      if (!string.IsNullOrWhiteSpace(ErrorMessage)) {
        throw new ApplicationException(ErrorMessage);
      }
      BackupFolderPath = backupFolderPath;
    }
  }
}