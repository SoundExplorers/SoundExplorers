using System;
using System.Data;

namespace SoundExplorers.Data {
  public class SetList : EntityListBase<Set> {
    protected override void AddRowsToTable() {
      foreach (var set in this) {
        Table.Rows.Add(set.Event.Location.Name, set.Event.Date, set.SetNo, set.Act?.Name,
          set.Notes);
      }
    }

    protected override DataTable CreateEmptyTableWithColumns() {
      var result = new DataTable(nameof(Event.Date));
      result.Columns.Add(nameof(Location), typeof(string));
      result.Columns.Add(nameof(Event), typeof(DateTime));
      result.Columns.Add(nameof(Set.SetNo), typeof(int));
      result.Columns.Add(nameof(Act), typeof(string));
      result.Columns.Add(nameof(Set.Notes), typeof(string));
      return result;
    }
  }
}