using NUnit.Framework;
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
      Controller = new TestMainController(View, QueryHelper, Session);
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
      Assert.AreSame(Controller, View.Controller, "view.Controller");
      Controller.DatabaseConnection = Connection;
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
  }
}