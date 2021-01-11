using System.Collections;
using System.ComponentModel;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase(IEditorView editorView) {
      EditorView = editorView;
    }

    public IBindingList? BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    public IEditorView EditorView { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public virtual void Populate(IList? list = null) {
      List.Populate(list);
    }
  }
}