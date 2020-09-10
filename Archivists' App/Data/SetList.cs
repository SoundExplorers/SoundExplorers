using System;
using System.Collections.Generic;
using System.Data;

namespace SoundExplorers.Data {
  public class SetList : EntityListBase<Set> {
    /// <summary>
    ///   Creates a new instance of the <see cref="SetList" /> class.
    /// </summary>
    /// <param name="isParentListRequired">
    ///   Specifies whether an identifying parent Event list
    ///   is to be included.
    /// </param>
    public SetList(bool isParentListRequired) : base(isParentListRequired) { }

    protected override void AddRowsToTable() {
      foreach (var set in this) {
        Table.Rows.Add(set.Event.Location.Name, set.Event.Date, set.SetNo, set.Act?.Name,
          set.Notes);
      }
    }

    protected override IEntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Location), typeof(string),
          nameof(Event),
          nameof(Location)),
        new EntityColumn(nameof(Event), typeof(DateTime),
          nameof(Event),
          nameof(Event.Date)),
        new EntityColumn(nameof(Set.SetNo), typeof(int)),
        new EntityColumn(nameof(Act), typeof(string),
          nameof(Act), nameof(Act.Name)),
        new EntityColumn(nameof(Set.Notes), typeof(string))
      };
    }

    protected override IEntityList CreateParentList() {
      // The constructor indicates that the parent list
      // does not itself require a parent list.
      throw new NotImplementedException();
      //return new EventList(false);
    }

    protected override DataColumn[] GetPrimaryKeyDataColumns() {
      var list = new List<DataColumn> {
        Table.Columns[nameof(Location)],
        Table.Columns[nameof(Event)],
        Table.Columns[nameof(Set.SetNo)]
      };
      return list.ToArray();
    }
  }
}