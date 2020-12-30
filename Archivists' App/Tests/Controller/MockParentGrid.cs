using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockParentGrid : IParentGrid {
    public ParentGridController Controller { get; init; } = null!;
  }
}