namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    void OnError();
    void OnMainGridPopulated();
    void OnPopulated();
    void OnParentAndMainGridsShown();
    void PopulateMainGridOnParentRowChanged(int parentRowIndex);
    void Refresh();
    void SetCursorToDefault();
    void ShowErrorMessage(string text);
    void ShowWarningMessage(string text);
  }
}