using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    EditorController Controller { get; }
    // bool IsFocusingParentGrid { get; set; }
    bool IsPopulating { get; }
    IMainGrid MainGrid { get; }
    IParentGrid ParentGrid { get; }
    void OnError();
    void OnMainGridPopulated();
    void ShowErrorMessage([NotNull] string text);
    void ShowWarningMessage(string text);
  }
}