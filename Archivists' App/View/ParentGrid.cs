﻿using System;
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

    internal MainGrid MainGrid { get; set; }

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
      if (CurrentCell != null) {
        MainGrid.AutoResizeColumns();
      }
    }

    /// <summary>
    ///   An existing row on the parent grid has been entered.
    ///   So the main grid will be populated with the required
    ///   child entities of the entity at the specified row index.
    /// </summary>
    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      base.OnRowEnter(e);
      MainGrid.Populate(Controller.GetChildrenForMainList(e.RowIndex));
    }

    public override void Populate(IList list = null) {
      base.Populate(list);
      if (RowCount > 0) {
        // Make the last row current.
        // This triggers OnRowEnter, which will populate the main grid.
        CurrentCell = Rows[^1].Cells[0]; 
      }
    }
  }
}