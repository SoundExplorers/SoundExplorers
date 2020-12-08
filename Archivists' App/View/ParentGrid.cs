using System;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  public class ParentGrid : GridBase,  IView<ParentGridController> {
    public ParentGrid() {
      AllowUserToAddRows = false;
      AllowUserToDeleteRows = false;
      MultiSelect = false;
      ReadOnly = true;
    }
    
    private ParentGridController Controller { get; set; }
    internal MainGrid MainGrid { get; set; }

    public void SetController(ParentGridController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Resizes the main grid when its contents are automatically
    ///   kept in step with the parent grid row change.
    /// </summary>
    /// <remarks>
    ///   This really only needs to be done when the current row changes.
    ///   But there's no event for that.  The RowEnter event is raised
    ///   just before the row becomes current.  So it is too early
    ///   to work:  I tried.
    /// </remarks>
    protected override void OnCurrentCellChanged(EventArgs e) {
      base.OnCurrentCellChanged(e);
      MainGrid.AutoResizeColumns();
    }

    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      base.OnRowEnter(e);
      Controller.OnRowEnter(e.RowIndex);
    }
  }
}