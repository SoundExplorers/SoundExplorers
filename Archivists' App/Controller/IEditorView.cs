namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    IMainGrid MainGrid { get; }
    IParentGrid ParentGrid { get; }
    
    IGrid GetOtherGrid(IGrid grid);
    void OnError();
    void OnParentAndMainGridsShownAsync();
    void PopulateMainGridOnParentRowChanged(int parentRowIndex);
    void SetCursorToDefault();
    void ShowErrorMessage(string text);
    void ShowWarningMessage(string text);
  }
}