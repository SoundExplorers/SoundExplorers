using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public class MockMainGrid : MockGridBase, IMainGrid {
    public new TestMainGridController Controller {
      get => (TestMainGridController)base.Controller;
      set => base.Controller = value;
    }

    internal int EditCurrentCellCount { get; private set; }
    internal int MakeCellCurrentCount { get; private set; }
    internal int MakeCellCurrentColumnIndex { get; private set; }
    internal int MakeCellCurrentRowIndex { get; private set; }
    internal int OnRowAddedOrDeletedCount { get; private set; }
    internal int RestoreCurrentRowCellErrorValueCount { get; private set; }
    internal int SelectCurrentRowOnlyCount { get; private set; }

    /// <summary>
    ///   Gets the grid row count including the new row. In the application, RowCount is
    ///   a property of MainGrid's DataGridView base class, not derived from
    ///   EditorController.BindingList, as we have to do in this simulation. Because
    ///   MainGrid is a DataGridView that is not ReadOnly, there is a 'new row', which is
    ///   in the DataGridView but not the BindingList, unless the last DataGridView row
    ///   is current.  In the application, one use of RowCount is to make the new row
    ///   current, which adds it to the BindingList: see
    ///   <see cref="EditorController.OnPopulatedAsync" />. We have to support that here.
    ///   When the new row is no longer the current row, it is removed from the
    ///   BindingList.  
    ///   <para>
    ///     So the use of RowCount in tests should be avoided. Use
    ///     EditorController.BindingList.Count instead.
    ///   </para>
    /// </summary>
    public override int RowCount => Controller.BindingList!.Count + 1;

    public void SetController(MainGridController controller) {
      Controller = (TestMainGridController)controller;
    }

    public void EditCurrentCell() {
      EditCurrentCellCount++;
    }

    public void MakeCellCurrent(int rowIndex, int columnIndex) {
      MakeCellCurrentCount++;
      MakeCellCurrentColumnIndex = columnIndex;
      MakeCellCurrentRowIndex = rowIndex;
    }

    public void OnRowAddedOrDeleted() {
      OnRowAddedOrDeletedCount++;
    }

    public void RestoreCurrentRowCellErrorValue(int columnIndex,
      object? errorValue) {
      RestoreCurrentRowCellErrorValueCount++;
    }

    public void SelectCurrentRowOnly() {
      SelectCurrentRowOnlyCount++;
    }
  }
}