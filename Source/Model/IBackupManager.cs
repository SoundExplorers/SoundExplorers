namespace SoundExplorers.Model {
  public interface IBackupManager {
    string BackupFolderPath { get; }
    void BackupDatabaseTo(string backupFolderPath);
  }
}