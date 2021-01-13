﻿using System.Diagnostics;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal class ParentGrid : GridBase, IParentGrid, IView<ParentGridController> {
    public ParentGrid() {
      AllowUserToAddRows = false;
      AllowUserToDeleteRows = false;
      MultiSelect = false;
      ReadOnly = true;
    }

    public new ParentGridController Controller {
      get => (ParentGridController)base.Controller;
      private set => base.Controller = value;
    }

    public void SetController(ParentGridController controller) {
      Controller = controller;
    }

    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      Debug.WriteLine($"ParentGrid.OnRowEnter: row {e.RowIndex}");
      Controller.OnRowEnter(e.RowIndex);
    }
  }
}