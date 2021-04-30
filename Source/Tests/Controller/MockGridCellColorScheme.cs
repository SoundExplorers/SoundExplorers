using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockGridCellColorScheme : IGridCellColorScheme {
    internal int InvertCount { get; private set; }
    internal int RestoreToDefaultCount { get; private set; }

    public void Invert() {
      InvertCount++;
    }

    public void RestoreToDefault() {
      RestoreToDefaultCount++;
    }
  }
}