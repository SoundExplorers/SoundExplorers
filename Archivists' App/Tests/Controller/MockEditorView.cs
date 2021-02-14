using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockEditorView : IEditorView {
    public MockEditorView(IMainGrid mainGrid, IParentGrid parentGrid) {
      MainGrid = mainGrid;
      ParentGrid = parentGrid;
    }

    internal int CloseCount { get; private set; }
    internal string LastErrorMessage { get; private set; } = null!;
    internal string LastWarningMessage { get; private set; } = null!;
    internal TestMainGridController MainGridController { get; set; } = null!;
    internal int OnParentAndMainGridsShownAsyncCount { get; private set; }
    internal int SetMouseCursorToDefaultCount { get; private set; }
    internal int SetMouseCursorToWaitCount { get; private set; }
    internal int ShowErrorMessageCount { get; private set; }
    internal int ShowWarningMessageCount { get; private set; }

    // private EditorController Controller { get; set; } = null!;
    public IGrid? CurrentGrid { get; set; }
    public IMainGrid MainGrid { get; }
    public IParentGrid ParentGrid { get; }

    public void SetController(EditorController controller) {
      // Controller = controller;
    }

    public void Close() {
      CloseCount++;
    }

    public void OnError() {
      MainGridController.ShowError();
    }

    public void OnParentAndMainGridsShown() {
      OnParentAndMainGridsShownAsyncCount++;
    }

    // public void PopulateMainGridOnParentRowChanged(int parentRowIndex) {
    //   Controller.PopulateMainGridOnParentRowChanged(parentRowIndex);
    // }

    public void SetMouseCursorToDefault() {
      SetMouseCursorToDefaultCount++;
    }

    public void SetMouseCursorToWait() {
      SetMouseCursorToWaitCount++;
    }

    public void ShowErrorMessage(string text) {
      //Debug.WriteLine("MockEditorView.ShowErrorMessage");
      LastErrorMessage = text;
      ShowErrorMessageCount++;
    }

    public void ShowWarningMessage(string text) {
      LastWarningMessage = text;
      ShowWarningMessageCount++;
    }
  }
}