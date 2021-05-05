using System;

namespace SoundExplorers.Model {
  public interface IBackupManager {
    string BackupFolderPath { get; }
    DateTime LastBackupDateTime { get; }
    void BackupDatabaseTo(string backupFolderPath);
  }
}