using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface ITableView {
    void SetController([NotNull] TableController controller);
  }
}