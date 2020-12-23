using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  public class ParentGridController {
    public ParentGridController([NotNull] EditorController editorController) {
      EditorController = editorController;
    }

    private EditorController EditorController { get; }

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    public void OnRowEnter(int rowIndex) {
      EditorController.MainList.Populate(
        EditorController.ParentList?.GetChildrenForMainList(rowIndex));
    }
  }
}