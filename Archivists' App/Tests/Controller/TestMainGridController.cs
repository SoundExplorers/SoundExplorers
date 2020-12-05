using System.Data.Linq;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class TestMainGridController : MainGridController {
    public TestMainGridController([NotNull] IEditorView editorView) : base(editorView) { }
    public bool AutoValidate { get; set; }

    protected override ChangeAction LastChangeAction => TestUnsupportedLastChangeAction
      ? ChangeAction.None
      : base.LastChangeAction;

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