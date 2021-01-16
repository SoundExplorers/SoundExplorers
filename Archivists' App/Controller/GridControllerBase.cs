using System.Collections;
using System.ComponentModel;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase(EditorController editorController) {
      EditorController = editorController;
    }

    public IBindingList? BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }
    // public int FirstVisibleColumnIndex { get; private set; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public abstract void OnRowEnter(int rowIndex);

    public virtual void Populate(IList? list = null) {
      List.Populate(list);
      //   FirstVisibleColumnIndex = GetFirstVisibleColumnIndex();
    }

    // protected virtual int GetFirstVisibleColumnIndex() {
    //   return 0;
    // }
  }
}