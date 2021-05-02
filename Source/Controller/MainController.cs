using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  [UsedImplicitly]
  public class MainController {
    private IBackupManager? _backupManager;
    private IOpen? _databaseConnection;
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
    internal IOpen DatabaseConnection {
      get => _databaseConnection ?? new DatabaseConnection();
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

    private Option StatusBarOption => _statusBarOption ??=
      CreateOption("StatusBar", true);

    private Option TableOption => _tableOption ??= CreateOption("Table");

    private Option ToolBarOption => _toolBarOption ??=
      CreateOption("ToolBar", true);

    private IMainView View { get; }

    public void BackupDatabase() {
      string newBackupFolderPath =
        View.AskForBackupFolderPath(BackupManager.BackupFolderPath);
      if (string.IsNullOrWhiteSpace(newBackupFolderPath)) {
        View.SetStatusBarText("Database backup cancelled.");
        return;
      }
      View.SetMouseCursorToWait();
      View.SetStatusBarText("Backing up database. Please wait...");
      View.BeginInvoke(() => BackupDatabaseAsync(newBackupFolderPath));
    }

    public void ConnectToDatabase() {
      DatabaseConnection.Open();
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

    private void BackupDatabaseAsync(string backupFolderPath) {
      try {
        BackupManager.BackupDatabaseTo(backupFolderPath);
        const string confirmationMessage = "The database backup has completed.";
        View.SetStatusBarText(confirmationMessage);
        View.ShowInformationMessage(confirmationMessage);
      } catch (ApplicationException exception) {
        View.SetStatusBarText("The database backup failed.");
        View.ShowErrorMessage(exception.Message);
      } finally {
        View.SetMouseCursorToDefault();
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