using System;
using System.IO;
using NUnit.Framework;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class BackupManagerTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      DatabaseFolder = new DirectoryInfo(Session.SystemDirectory);
      BackupFolder = CreateTestFolder("Backup");
      RestoreFolder = CreateTestFolder("Restore");
      Session.BeginUpdate();
      Data.AddRolesPersisted(1, Session);
      Session.Commit();
      BackupManager = new TestBackupManager(QueryHelper, Session);
    }

    [TearDown]
    public override void TearDown() {
      base.TearDown();
      TestSession.DeleteFolderIfExists(BackupFolder.FullName);
      TestSession.DeleteFolderIfExists(RestoreFolder.FullName);
    }

    private DirectoryInfo BackupFolder { get; set; } = null!;
    private TestBackupManager BackupManager { get; set; } = null!;
    private DirectoryInfo DatabaseFolder { get; set; } = null!;
    private DirectoryInfo RestoreFolder { get; set; } = null!;

    [Test]
    public void BackupAndRestore() {
      Assert.AreEqual(string.Empty, BackupManager.BackupFolderPath,
        "BackupFolderPath before backup");
      Assert.IsTrue(BackupManager.IsTimeToPromptForBackup,
        "IsTimeToPromptForBackup initially");
      Assert.AreEqual("Would you like to back up the database now?",
        BackupManager.PromptForBackupQuestion, "PromptForBackupQuestion initially");
      BackupManager.TestBackupDateTime = DateTime.Parse("2099/12/31 23:59:59");
      BackupManager.BackupDatabaseTo(BackupFolder.FullName);
      Assert.IsFalse(BackupManager.IsTimeToPromptForBackup,
        "IsTimeToPromptForBackup after backup");
      Assert.AreEqual(
        "Would you like to back up the database now?\r\n\r\n" +
        "The database was last backed up on 31 December 2099 at 23:59:59.",
        BackupManager.PromptForBackupQuestion, "PromptForBackupQuestion after backup");
      Assert.AreEqual(BackupFolder.FullName, BackupManager.BackupFolderPath,
        "BackupFolderPath after backup");
      var backupFiles = BackupFolder.GetFiles();
      Assert.AreEqual(1, backupFiles.Length, "Backup file count");
      Assert.AreEqual("Backup20991231235959.zip", backupFiles[0].Name,
        "Backup file name");
      SoundExplorers.Model.BackupManager.UnzipToFolder(
        backupFiles[0].FullName, RestoreFolder.FullName);
      Assert.AreEqual(DatabaseFolder.GetFiles().Length, RestoreFolder.GetFiles().Length,
        "Restored file count");
    }

    [Test]
    public void InvalidBackupFolder() {
      string invalidPath = BackupFolder.FullName + "Z";
      var exception =
        Assert.Catch<ApplicationException>(() =>
          BackupManager.BackupDatabaseTo(invalidPath), 
          "Disallow non-existent backup folder");
      Assert.AreEqual($"Backup folder '{invalidPath}' does not exist.", 
        exception!.Message, "Non-existent backup folder message");
    }

    private DirectoryInfo CreateTestFolder(string prefix) {
      var result = new DirectoryInfo(Path.Combine(TestSession.DatabaseParentFolderPath,
        DatabaseFolder.Name.ToLower().Replace("database", prefix)));
      result.Create();
      return result;
    }
  }
}