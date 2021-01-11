using System;
using System.Collections;
using System.Diagnostics;
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
    
    private int PreviousRowIndex { get; set; }

    public void SetController(ParentGridController controller) {
      Controller = controller;
    }

    protected override void OnGotFocus(EventArgs e) {
      Debug.WriteLine("ParentGrid.OnGotFocus");
      base.OnGotFocus(e);
    }

    /// <summary>
    ///   An existing row on the parent grid has been entered. So the main grid will be
    ///   populated with the required child entities of the entity at the specified row
    ///   index.
    /// </summary>
    protected override void OnRowEnter(DataGridViewCellEventArgs e) {
      Debug.WriteLine($"ParentGrid.OnRowEnter: row {e.RowIndex}");
      Debug.WriteLine($"    PreviousRowIndex = {PreviousRowIndex}; IsPopulating = {EditorView.IsPopulating}");
      base.OnRowEnter(e);
      if (EditorView.IsPopulating || e.RowIndex == PreviousRowIndex) {
        PreviousRowIndex = e.RowIndex;
        return;
      }
      EditorView.PopulateMainGridOnParentRowChanged(e.RowIndex);
      PreviousRowIndex = e.RowIndex;
    }

    public override void Populate(IList? list = null) {
      Debug.WriteLine("ParentGrid.Populate");
      PreviousRowIndex = -1;
      base.Populate(list);
      Debug.WriteLine("ParentGrid.Populate: END");
    }
  }
}