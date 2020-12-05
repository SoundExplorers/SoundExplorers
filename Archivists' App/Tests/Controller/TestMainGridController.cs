using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class TestMainGridController : MainGridController {
    // ReSharper disable once SuggestBaseTypeForParameter (See MockEditorView property)
    public TestMainGridController([NotNull] MockEditorView editorView) :
      base(editorView) { }

    public bool AutoValidate { get; set; }

    protected override ChangeAction LastChangeAction => TestUnsupportedLastChangeAction
      ? ChangeAction.None
      : base.LastChangeAction;

    [NotNull] public MockEditorView MockEditorView => (MockEditorView)EditorView;
    public bool TestUnsupportedLastChangeAction { get; set; }

    public override void OnRowEnter(int rowIndex) {
      if (AutoValidate) {
        if (List.HasRowBeenEdited) {
          OnRowValidated(CurrentRowIndex);
        }
        if (rowIndex == List.BindingList.Count) { // New row
          List.BindingList.AddNew();
        }
      }
      base.OnRowEnter(rowIndex);
    }
  }
}