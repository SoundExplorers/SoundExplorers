using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public class SetList : List<Set> {
    private SessionBase _session;
    public SetList() {
      Table = CreateEmptyTableWithColumns();
    }

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    [NotNull]
    public DataTable Table { get;}

    [NotNull]
    internal SessionBase Session {
      get => _session ?? (_session = Global.Session);
      set => _session = value;
    }

    /// <summary>
    ///   Fetches the required entities from the database
    ///   and populates the list and table with them.
    /// </summary>
    public void Fetch() {
      Clear();
      AddRange(Session.AllObjects<Set>());
      Table.Clear();
    }

    private static DataTable CreateEmptyTableWithColumns() {
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