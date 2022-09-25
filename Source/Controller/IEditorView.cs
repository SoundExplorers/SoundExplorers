namespace SoundExplorers.Controller; 

public interface IEditorView : IView<EditorController> {
  IGrid? CurrentGrid { get; }
  IMainGrid MainGrid { get; }
  IParentGrid ParentGrid { get; }
  void Close();
  void OnError();
  void OnParentAndMainGridsShown();
  void SetMouseCursorToDefault();
  void SetMouseCursorToWait();
  void SetStatusBarText(string text);
  void ShowErrorMessage(string text);
  void ShowWarningMessage(string text);
}