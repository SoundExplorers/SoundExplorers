using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  public class SetList : EntityListBase<Set> {
    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   so that a subsequent call of the Fetch method will populate
    ///   the instance's list with all the Set records on the database.
    /// </summary>
    /// <remarks>
    ///   Used when Set is the main table.
    /// </remarks>
    [UsedImplicitly]
    public SetList()
      : base(typeof(EventList)) { }
    
    /// <summary>
    ///   Initialises a new instance of the <see cref="SetList" /> class,
    ///   so that a subsequent call of the Fetch method will populate
    ///   the instance's list with all the Set records on the database.
    /// </summary>
    /// <remarks>
    ///   Used when Set is the parent table.
    /// </remarks>
    [UsedImplicitly]
    public SetList(object[] dummy = null) { }

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