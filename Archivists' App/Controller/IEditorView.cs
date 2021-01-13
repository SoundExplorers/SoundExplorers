using System;

namespace SoundExplorers.Controller {
  public interface IEditorView : IView<EditorController> {
    void AsyncInvoke(Action method);
    void OnParentAndMainGridsShown();
    void SetCursorToDefault();
  }
}