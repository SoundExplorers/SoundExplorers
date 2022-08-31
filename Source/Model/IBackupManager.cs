using System;

namespace SoundExplorers.Model {
  public interface IBackupManager {
    string BackupFolderPath { get; }
    bool IsTimeToPromptForBackup { get; }
    DateTime LastBackupDateTime { get; }
    string PromptForBackupQuestion { get; }
    void BackupDatabaseTo(string backupFolderPath);
  }
}