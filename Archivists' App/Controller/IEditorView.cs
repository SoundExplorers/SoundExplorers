using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    EditorController Controller { get; }
    void OnError();
    void ShowErrorMessage([NotNull] string text);
    void ShowWarningMessage(string text);
  }
}