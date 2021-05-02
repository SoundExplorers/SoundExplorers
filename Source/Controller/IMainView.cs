using System;

namespace SoundExplorers.Controller {
  public interface IMainView : IView<MainController> {
    string AskForBackupFolderPath(string previousPath);
    void BeginInvoke(Action action);
    void SetMouseCursorToDefault();
    void SetMouseCursorToWait();
    void SetStatusBarText(string text);
    void ShowErrorMessage(string text);
    void ShowInformationMessage(string text);
  }
}