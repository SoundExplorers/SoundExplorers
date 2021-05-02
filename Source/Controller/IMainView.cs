namespace SoundExplorers.Controller {
  public interface IMainView : IView<MainController> {
    string AskForBackupFolderPath(string previousPath);
    void SetMouseCursorToDefault();
    void SetMouseCursorToWait();
    void SetStatusBarText(string text);
    void ShowErrorMessage(string text);
  }
}