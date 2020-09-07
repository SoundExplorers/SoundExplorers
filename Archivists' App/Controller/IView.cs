using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IView<in TController> {
    void SetController([NotNull] TController controller);
  }
}