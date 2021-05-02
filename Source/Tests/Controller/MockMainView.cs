using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainView : MockView<MainController>, IMainView {
    internal int AskForBackupFolderPathCount { get; private set; }
    internal string LastErrorMessage { get; private set; } = null!;
    internal string BackupFolderPath { get; set; }
    internal string PreviousPath { get; private set; } = null!;
    internal int SetMouseCursorToDefaultCount { get; private set; }
    internal int SetMouseCursorToWaitCount { get; private set; }
    internal int ShowErrorMessageCount { get; private set; }
    internal string StatusBarText { get; private set; } = null!;
    
    public string AskForBackupFolderPath(string previousPath) {
      PreviousPath = previousPath;
      AskForBackupFolderPathCount++;
      return BackupFolderPath;
    }

    public void SetMouseCursorToDefault() {
      SetMouseCursorToDefaultCount++;
    }

    public void SetMouseCursorToWait() {
      SetMouseCursorToWaitCount++;
    }

    public void SetStatusBarText(string text) {
      StatusBarText = text;
    }

    public void ShowErrorMessage(string text) {
      LastErrorMessage = text;
      ShowErrorMessageCount++;
    }
  }
}