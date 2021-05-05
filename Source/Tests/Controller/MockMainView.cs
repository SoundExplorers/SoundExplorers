using System;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainView : MockView<MainController>, IMainView {
    internal int AskForBackupFolderPathCount { get; private set; }
    internal int AskOkCancelQuestionCount { get; private set; }
    internal int CloseCount { get; private set; }
    internal string LastInformationMessage { get; private set; } = null!;
    internal string LastErrorMessage { get; private set; } = null!;
    internal string BackupFolderPath { get; set; } = null!;
    internal bool OkCancelAnswer { get; set; }
    internal string PreviousBackupFolderPath { get; private set; } = null!;
    internal int SetMouseCursorToDefaultCount { get; private set; }
    internal int SetMouseCursorToWaitCount { get; private set; }
    internal int ShowErrorMessageCount { get; private set; }
    internal int ShowInformationMessageCount { get; private set; }
    internal string StatusBarText { get; private set; } = null!;
    
    public string AskForBackupFolderPath(string previousPath) {
      PreviousBackupFolderPath = previousPath;
      AskForBackupFolderPathCount++;
      return BackupFolderPath;
    }

    public bool AskOkCancelQuestion(string text) {
      AskOkCancelQuestionCount++;
      return OkCancelAnswer;
    }

    void IMainView.BeginInvoke(Action action) {
      action.Invoke();
    }

    public void Close() {
      CloseCount++;
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

    public void ShowInformationMessage(string text) {
      LastInformationMessage = text;
      ShowInformationMessageCount++;
    }
  }
}