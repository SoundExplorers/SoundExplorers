using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    public string LastErrorMessage { get; private set; } = null!;
    public TestMainGridController MainGridController { get; set; } = null!;
    public int SetCursorToDefaultCount { get; private set; }
    public int ShowErrorMessageCount { get; private set; }
    public int ShowWarningMessageCount { get; private set; }
    private EditorController Controller { get; set; } = null!;

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    public void OnError() {
      MainGridController.ShowError();
    }

    public void OnMainGridPopulated() {
      Controller.OnMainGridPopulatedAsync();
    }

    public void OnPopulated() {
      Controller.OnPopulatedAsync();
    }

    public void OnParentAndMainGridsShown() {
      Controller.OnParentAndMainGridsShownAsync();
    }

    public void PopulateMainGridOnParentRowChanged(int parentRowIndex) {
      Controller.PopulateMainGridOnParentRowChanged(parentRowIndex);
    }

    public void Refresh() {
      throw new System.NotImplementedException();
    }

    public void SetCursorToDefault() {
      SetCursorToDefaultCount++;
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