using System;
using System.Collections;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public class ParentGridController : GridControllerBase {
    public ParentGridController([NotNull] IEditorView editorView) : base(editorView) { }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected override IEntityList List => EditorView.Controller.ParentList;
    
    [NotNull]
    public IList GetChildrenForMainList(int rowIndex) {
      return
        List.GetChildrenForMainList(rowIndex)
        ?? throw new InvalidOperationException(
          $"List.GetChildrenForMainList({rowIndex}) unexpectedly returns null.");
    }
  }
}