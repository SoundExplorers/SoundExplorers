namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    EditorController Controller { get; }

    IMainGrid MainGrid { get; }
    IParentGrid ParentGrid { get; }
    void OnError();
    void ShowErrorMessage(string text);
    void ShowWarningMessage(string text);
  }
}