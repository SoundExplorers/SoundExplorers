using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller; 

public class MockView<TController> : IView<TController> {
  public TController Controller { get; private set; } = default!;

  public void SetController(TController controller) {
    Controller = controller;
  }
}