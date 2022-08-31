using System;

namespace SoundExplorers.Controller {
  public interface IMainView : IView<MainController> {
    string AskForBackupFolderPath(string previousPath);
    bool AskOkCancelQuestion(string text);
    bool AskYesNoQuestion(string text);
    void BeginInvoke(Action action);
    void Close();
    void SetMouseCursorToDefault();
    void SetMouseCursorToWait();
    void SetStatusBarText(string text);
    void ShowErrorMessage(string text);
    void ShowInformationMessage(string text);
  }
}