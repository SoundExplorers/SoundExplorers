using System;
using SoundExplorers.Model;

namespace SoundExplorers.Tests.Model {
  public class MockBackupManager : IBackupManager {
    internal string ErrorMessage { get; set; } = null!;
    public string BackupFolderPath { get; internal set; } = null!;
    public bool IsTimeToPromptForBackup { get; internal set; }
    public DateTime LastBackupDateTime { get; internal set; }
    public string PromptForBackupQuestion => "What's the difference between a duck?";

    public void BackupDatabaseTo(string backupFolderPath) {
      if (!string.IsNullOrWhiteSpace(ErrorMessage)) {
        throw new ApplicationException(ErrorMessage);
      }
      BackupFolderPath = backupFolderPath;
    }
  }
}