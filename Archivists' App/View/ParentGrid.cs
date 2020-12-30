using System;
using System.Collections;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class ParentGrid : GridBase, IView<ParentGridController> {
    public ParentGrid() {
      AllowUserToAddRows = false;
      AllowUserToDeleteRows = false;
      MultiSelect = false;
      ReadOnly = true;
    }

    private new ParentGridController Controller {
      get => (ParentGridController)base.Controller;
      set => base.Controller = value;
    }

    public void SetController(ParentGridController controller) {
      Controller = controller;
    }

    protected override void ConfigureColumns() {
      foreach (DataGridViewColumn column in Columns) {
        if (column.ValueType == typeof(DateTime)) {
          column.DefaultCellStyle.Format = EditorController.DateFormat;
        }
      } // End of foreach
    }

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      base.OnRowEnter(e);
      Controller.OnRowEnter(e.RowIndex);
    }

    public override void Populate(IList? list = null) {
      base.Populate(list);
      if (RowCount > 0) {
        // Make the last row current.
        // This triggers OnRowEnter, which will populate the main grid.
        CurrentCell = Rows[^1].Cells[0];
      }
    }
  }
}