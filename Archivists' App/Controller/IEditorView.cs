namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    IMainGrid MainGrid { get; }
    IParentGrid ParentGrid { get; }
    
    void OnError();
    void OnParentAndMainGridsShownAsync();
    void PopulateMainGridOnParentRowChanged(int parentRowIndex);
    void SetCursorToDefault();
    void SetCursorToWait();
    void ShowErrorMessage(string text);
    void ShowWarningMessage(string text);
  }
}