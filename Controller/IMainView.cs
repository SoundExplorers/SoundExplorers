namespace SoundExplorers.Controller {
  public interface IMainView : IView<MainController> {
    void ShowErrorMessage(string text);
  }
}