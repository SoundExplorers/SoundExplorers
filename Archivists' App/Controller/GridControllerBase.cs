using System.Collections;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase([NotNull] IEditorView editorView) {
      EditorView = editorView;
    }
    
    [CanBeNull] public IBindingList BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's field properties.
    /// </summary>
    [NotNull]
    internal BindingColumnList Columns => List.Columns;
    
    [NotNull] public IEditorView EditorView { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }
    
    protected bool IsPopulating { get; private set; }
    protected int PreviousRowIndex { get; set; }

    [NotNull]
    public string GetColumnDisplayName([NotNull] string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public virtual void OnRowEnter(int rowIndex) {
      if (IsPopulating && rowIndex == List.Count -1) {
        IsPopulating = false;
      }
    }

    public void Populate(IList list = null) {
      IsPopulating = true;
      PreviousRowIndex = -1;
      List.Populate(list);
    }
  }
}