using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    internal string LastErrorMessage { get; private set; } = null!;
    internal TestMainGridController MainGridController { get; set; } = null!;
    internal int RefreshCount { get; private set; }
    internal int SetCursorToDefaultCount { get; private set; }
    internal int ShowErrorMessageCount { get; private set; }
    internal int ShowWarningMessageCount { get; private set; }
    internal EditorController Controller { get; set; } = null!;

    public void SetController(EditorController controller) {
      Controller = controller;
    }

    public void OnError() {
      MainGridController.ShowError();
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
      RefreshCount++;
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