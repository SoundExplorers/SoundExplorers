using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class MainController {
    private IBackupManager? _backupManager;
    private IDatabaseConnection? _databaseConnection;
    private Option? _statusBarOption;
    private Option? _tableOption;
    private Option? _toolBarOption;

    public MainController(IMainView view) {
      View = view;
      View.SetController(this);
    }

    /// <summary>
    ///   The database connection, whose default should only need to be replaced in
    ///   tests.
    /// </summary>
    internal IDatabaseConnection DatabaseConnection {
      get => _databaseConnection ??= new DatabaseConnection();
      set => _databaseConnection = value;
    }

    public bool IsClosing { get; set; }

    public bool IsStatusBarVisible {
      get => StatusBarOption.BooleanValue;
      set => StatusBarOption.BooleanValue = value;
    }

    public bool IsToolBarVisible {
      get => ToolBarOption.BooleanValue;
      set => ToolBarOption.BooleanValue = value;
    }

    public string TableName {
      get => TableOption.StringValue;
      set => TableOption.StringValue = value;
    }

    private IBackupManager BackupManager => _backupManager ??= CreateBackupManager();
    private  bool MustBackup { get; set; }

    private Option StatusBarOption => _statusBarOption ??=
      CreateOption("StatusBar", true);

    private Option TableOption => _tableOption ??= CreateOption("Table");

    private Option ToolBarOption => _toolBarOption ??=
      CreateOption("ToolBar", true);

    private IMainView View { get; }

    public void BackupDatabase() {
      if (MustBackup) {
        if (!View.AskOkCancelQuestion(
          "A database schema upgrade is pending. " + 
          "So you must first back up the database.\r\n\r\n" + 
          "Answer OK to back up the database now.\r\n" 
          + $"Answer Cancel to close {GetProductName()}.")) {
          View.Close();
          return;
        }
      }
      string newBackupFolderPath =
        View.AskForBackupFolderPath(BackupManager.BackupFolderPath);
      if (string.IsNullOrWhiteSpace(newBackupFolderPath)) {
        if (MustBackup) {
          View.Close();
        } else {
          View.SetStatusBarText("Database backup cancelled.");
        }
        return;
      }
      View.SetStatusBarText("Backing up database. Please wait...");
      View.SetMouseCursorToWait();
      // Running the backup in a thread rather than via View.BeginInvoke fixes a problem
      // where, though the wait cursor was shown while the backup was running, the status
      // message was not.
      var backupThread =
        new Thread(() => BackupDatabaseAsync(newBackupFolderPath)) {
          Name = nameof(BackupDatabaseAsync)
        };
      backupThread.Start();
    }

    public void ConnectToDatabase() {
      DatabaseConnection.Open();
      MustBackup = DatabaseConnection.MustBackup;
    }

    public void OnWindowShown() {
      if (MustBackup) {
        View.BeginInvoke(BackupDatabase);
      }
    }

    [ExcludeFromCodeCoverage]
    public void ShowEntityRelationshipDiagram() {
      ShowHelpFile("Entity Relationship Diagram.pdf");
    }

    [ExcludeFromCodeCoverage]
    public void ShowKeyboardShortcuts() {
      ShowHelpFile("Keyboard Shortcuts.pdf");
    }

    /// <summary>
    ///   Opens the specified file in the default application for its extension.
    /// </summary>
    /// <remarks>
    ///   This works for Windows. It may or may not for Mac and Linux, when and if we
    ///   ever decide to support either of them. See
    ///   https://github.com/dotnet/runtime/issues/17938 for discussion and suggestions
    ///   for those operating systems.
    /// </remarks>
    /// <param name="fileName">
    ///   The name of the file to open. A URL if a link is to be opened in the default
    ///   web browser.
    /// </param>
    [ExcludeFromCodeCoverage]
    internal static void OpenFile(string fileName) {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        Process.Start(new ProcessStartInfo {
          FileName = fileName,
          UseShellExecute = true
        });
      } else {
        throw new NotSupportedException(
          "This action is not (yet) supported for " +
          $"{RuntimeInformation.OSDescription}");
      }
    }

    [ExcludeFromCodeCoverage]
    protected virtual IBackupManager CreateBackupManager() {
      return new BackupManager(QueryHelper.Instance, Global.Session);
    }

    [ExcludeFromCodeCoverage]
    protected virtual Option CreateOption(string name,
      object? defaultValue = null) {
      return new Option(name, defaultValue);
    }

    [ExcludeFromCodeCoverage]
    protected virtual string GetProductName() {
      return Global.GetProductName();
    }

    private void BackupDatabaseAsync(string backupFolderPath) {
      try {
        BackupManager.BackupDatabaseTo(backupFolderPath);
        const string confirmationMessage = "The database backup has completed.";
        View.SetStatusBarText(confirmationMessage);
        string informationMessage = confirmationMessage;
        if (MustBackup) {
          informationMessage += 
            $"\r\n\r\n{GetProductName()} will now close. " +
            "when you restart the application, the database schema will be upgraded.";
        }
        View.ShowInformationMessage(informationMessage);
      } catch (ApplicationException exception) {
        View.SetStatusBarText("The database backup failed.");
        View.ShowErrorMessage(exception.Message);
      } finally {
        if (MustBackup) {
          View.Close();
        } else {
          View.SetMouseCursorToDefault();
        }
      }
    }

    [ExcludeFromCodeCoverage]
    private void ShowHelpFile(string fileName) {
      string path = Path.Combine(
        Global.GetApplicationFolderPath(), "Documentation", fileName);
      if (File.Exists(path)) {
        OpenFile(path);
      } else {
        View.ShowErrorMessage($"Cannot find file '{path}'.");
      }
    }
  }
}