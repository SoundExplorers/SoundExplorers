using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainView : MockView<MainController>, IMainView {
    [ExcludeFromCodeCoverage]
    public void ShowErrorMessage(string text) { }
  }
}