using NUnit.Framework;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  [TestFixture]
  public class MainControllerTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      Connection = new MockDatabaseConnection();
      View = new MockMainView();
      Controller =
        new TestMainController(View, QueryHelper, Session) {
          DatabaseConnection = Connection
        };
    }

    private MockDatabaseConnection Connection { get; set; } = null!;
    private TestMainController Controller { get; set; } = null!;
    private MockMainView View { get; set; } = null!;

    [Test]
    public void BackupDatabase() {
      const string initialBackupFolderPath = "abc";
      Controller.MockBackupManager.BackupFolderPath = initialBackupFolderPath;
      View.BackupFolderPath = "def";
      Controller.BackupDatabase();
      TestHelper.WaitUntil(() => View.SetMouseCursorToDefaultCount == 1,
        "Backup finished");
      Assert.AreEqual("The database backup has completed.", View.StatusBarText,
        "StatusBarText");
      Assert.AreEqual(initialBackupFolderPath,
        View.PreviousBackupFolderPath, "PreviousBackupFolderPath");
      Assert.AreEqual(View.BackupFolderPath,
        Controller.MockBackupManager.BackupFolderPath, "BackupFolderPath");
      Assert.AreEqual(1, View.ShowInformationMessageCount, "ShowInformationMessageCount");
      Assert.AreEqual("The database backup has completed.", View.LastInformationMessage,
        "LastInformationMessage");
      Assert.AreEqual(1, View.SetMouseCursorToWaitCount, "SetMouseCursorToWaitCount");
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount");
    }

    [Test]
    public void BackupDatabaseWhenDue() {
      Connection.SchemaUpgradeStatus = SchemaUpgradeStatus.None;
      Controller.ConnectToDatabase();
      Controller.MockBackupManager.IsTimeToPromptForBackup = true;
      View.YesNoAnswer = true;
      View.BackupFolderPath = "def";
      Controller.OnWindowShown();
      TestHelper.WaitUntil(() => View.SetMouseCursorToDefaultCount == 1,
        "Backup finished");
    }

    [Test]
    public void CancelDatabaseBackup() {
      const string initialBackupFolderPath = "abc";
      Controller.MockBackupManager.BackupFolderPath = initialBackupFolderPath;
      View.BackupFolderPath = string.Empty;
      Controller.BackupDatabase();
      Assert.AreEqual("Database backup cancelled.", View.StatusBarText, "StatusBarText");
      Assert.AreEqual(initialBackupFolderPath,
        Controller.MockBackupManager.BackupFolderPath, "BackupFolderPath");
    }

    [Test]
    public void DatabaseBackupError() {
      const string errorMessage = "This is a test error message.";
      const string initialBackupFolderPath = "abc";
      Controller.MockBackupManager.BackupFolderPath = initialBackupFolderPath;
      Controller.MockBackupManager.ErrorMessage = errorMessage;
      View.BackupFolderPath = "def";
      Controller.BackupDatabase();
      TestHelper.WaitUntil(() => View.SetMouseCursorToDefaultCount == 1,
        "Backup stopped");
      Assert.AreEqual("The database backup failed.", View.StatusBarText,
        "StatusBarText");
      Assert.AreEqual(initialBackupFolderPath,
        Controller.MockBackupManager.BackupFolderPath, "BackupFolderPath");
      Assert.AreEqual(1, View.ShowErrorMessageCount, "ShowErrorMessageCount");
      Assert.AreEqual(errorMessage, View.LastErrorMessage, "LastErrorMessage");
      Assert.AreEqual(1, View.SetMouseCursorToWaitCount, "SetMouseCursorToWaitCount");
      Assert.AreEqual(1, View.SetMouseCursorToDefaultCount,
        "SetMouseCursorToDefaultCount");
    }

    [Test]
    public void MainTest() {
      const string tableName = "Set";
      Assert.AreSame(Controller, View.Controller, "View.Controller");
      Controller.ConnectToDatabase();
      Assert.AreEqual(1, Connection.OpenCount, "OpenCount");
      Assert.IsTrue(Controller.IsToolBarVisible, "IsToolBarVisible initially");
      Assert.IsTrue(Controller.IsStatusBarVisible, "IsStatusBarVisible initially");
      Controller.IsToolBarVisible = false;
      Controller.IsStatusBarVisible = false;
      Controller.TableName = tableName;
      Controller = new TestMainController(View, QueryHelper, Session);
      Assert.IsFalse(Controller.IsToolBarVisible, "IsToolBarVisible");
      Assert.IsFalse(Controller.IsStatusBarVisible, "IsStatusBarVisible");
      Assert.AreEqual(tableName, Controller.TableName, "TableName");
    }

    [Test]
    public void SchemaUpgradeComplete() {
      Connection.SchemaUpgradeStatus = SchemaUpgradeStatus.Complete;
      Controller.ConnectToDatabase();
      Controller.OnWindowShown();
      Assert.AreEqual(1, View.ShowInformationMessageCount, 
        "ShowInformationMessageCount");
      Assert.AreEqual("The database schema upgrade has completed.", 
        View.LastInformationMessage, "LastInformationMessage");
      Assert.AreEqual("The database schema upgrade has completed.", View.StatusBarText, 
        "StatusBarText");
    }

    [Test]
    public void SchemaUpgradePendingBackup() {
      Connection.SchemaUpgradeStatus = SchemaUpgradeStatus.Pending;
      Controller.ConnectToDatabase();
      View.OkCancelAnswer = true;
      View.BackupFolderPath = "def";
      Controller.OnWindowShown();
      TestHelper.WaitUntil(() => View.CloseCount == 1,
        "Backup finished and application closed");
    }

    [Test]
    public void SchemaUpgradePendingCancelBackup() {
      Connection.SchemaUpgradeStatus = SchemaUpgradeStatus.Pending;
      Controller.ConnectToDatabase();
      View.OkCancelAnswer = false;
      View.BackupFolderPath = "def";
      Controller.OnWindowShown();
      Assert.AreEqual(1, View.CloseCount);
    }

    [Test]
    public void SchemaUpgradePendingCancelSelectBackupFolder() {
      Connection.SchemaUpgradeStatus = SchemaUpgradeStatus.Pending;
      Controller.ConnectToDatabase();
      View.OkCancelAnswer = true;
      View.BackupFolderPath = string.Empty;
      Controller.OnWindowShown();
      TestHelper.WaitUntil(() => View.CloseCount == 1,
        "Backup finished and application closed");
    }
  }
}