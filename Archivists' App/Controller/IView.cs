namespace SoundExplorers.Controller {
  public interface IView<in TController> {
    void SetController(TController controller);
  }
}