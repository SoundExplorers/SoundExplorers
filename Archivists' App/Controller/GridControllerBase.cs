using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase(IGrid grid, EditorController editorController) {
      Grid = grid;
      EditorController = editorController;
    }

    public IBindingList? BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns represented by the Entity's field
    ///   properties.
    /// </summary>
    internal BindingColumnList Columns => List.Columns;

    protected EditorController EditorController { get; }
    
    /// <summary>
    ///   The origin of a request to focus the grid.
    /// </summary>
    /// <remarks>
    ///   All focus requests involve receipt of a Windows message. But only in two cases,
    ///   a mouse left-click or the application being brought into the foreground, is the
    ///   Windows message the origin of the focus request from the perspective of the
    ///   application.  In the other cases, mouse right-click and keyboard shortcut, the
    ///   focus request has to be initiated programmatically, i.e within the application.
    ///   The order in which information we need in order to prepare to switch focus
    ///   between the main grid and the parent grid, if there is one, depends on whether
    ///   the focus request has been initiated programmatically or has been first known
    ///   on receipt of the Windows message.
    /// </remarks>
    public FocusRequestOrigin FocusRequestOrigin { get; set; }
    
    protected IGrid Grid { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }

    /// <summary>
    ///   Gets whether the grid is being focused programatically, i.e. via
    ///   <see cref="IGrid.SetFocus" />. If false, either the grid is not being focused,
    ///   or it is being focusing by a left mouse button click or by the application
    ///   being brought into the foreground.
    /// </summary>
    protected bool IsFocusingProgramatically =>
      FocusRequestOrigin == FocusRequestOrigin.KeyboardShortcut ||
      FocusRequestOrigin == FocusRequestOrigin.MouseRightClick; 
    
    protected bool IsPopulating { get; private set; }

    public string GetColumnDisplayName(string columnName) {
      var column = Columns[columnName];
      return column.DisplayName ?? columnName;
    }

    public IGrid GetOtherGrid() {
      return Grid == EditorController.View.MainGrid
        ? EditorController.View.ParentGrid
        : EditorController.View.MainGrid;
    }

    public void OnFocusing() {
      Debug.WriteLine($"GridControllerBase.OnFocusing {Grid.Name}: FocusRequestOrigin = {FocusRequestOrigin}");
      if (FocusRequestOrigin == FocusRequestOrigin.WindowsMessage) {
        PrepareForFocus();
      }
      if (!IsPopulating && EditorController.IsParentGridToBeShown) {
        Grid.CellColorScheme.RestoreToDefault();
        GetOtherGrid().CellColorScheme.Invert();
      }
    }
    
    public virtual void OnGotFocus() {
      Debug.WriteLine("GridControllerBase.OnGotFocus");
      FocusRequestOrigin = FocusRequestOrigin.NotSpecified;
      if (!IsPopulating) {
        EditorController.View.SetMouseCursorToDefault();
      }
    }

    public abstract void OnRowEnter(int rowIndex);

    public virtual void OnPopulatedAsync() {
      Debug.WriteLine($"GridControllerBase.OnPopulatedAsync {Grid.Name}");
      IsPopulating = false;
      if (Grid.RowCount > 1) {
        Grid.MakeRowCurrent(Grid.RowCount - 1);
      }
    }

    public virtual void PrepareForFocus() {
      Debug.WriteLine($"GridControllerBase.PrepareForFocus {Grid.Name}: FocusRequestOrigin = {FocusRequestOrigin}");
      if (!IsPopulating) {
        EditorController.View.SetMouseCursorToWait();
      }
    }

    public virtual void Populate(IList? list = null) {
      Debug.WriteLine($"GridControllerBase.Populate {Grid.Name}");
      IsPopulating = true;
      List.Populate(list);
      Grid.OnPopulated();
    }
  }
}