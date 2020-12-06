using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    public string LastErrorMessage { get; private set; }
    public TestMainGridController MainGridController { get; set; }
    public int ShowErrorMessageCount { get; private set; }
    public int ShowWarningMessageCount { get; private set; }
    public EditorController Controller { get; private set; }

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    public void OnError() {
      MainGridController.ShowError();
    }

    public void ShowErrorMessage(string text) {
      //Debug.WriteLine("MockEditorView.ShowErrorMessage");
      LastErrorMessage = text;
      ShowErrorMessageCount++;
    }

    public void ShowWarningMessage(string text) {
      ShowWarningMessageCount++;
    }
  }
}