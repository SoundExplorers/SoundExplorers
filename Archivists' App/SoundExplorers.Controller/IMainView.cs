using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IMainView {
    void SetController([NotNull] MainController controller);
  }
}