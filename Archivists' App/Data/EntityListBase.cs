using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  public abstract class EntityListBase<TEntity> : List<TEntity>
    where TEntity : EntityBase {
    private SessionBase _session;
    private DataTable _table;

    /// <summary>
    ///   Gets the data table representing the list of entities.
    /// </summary>
    [NotNull]
    public DataTable Table => _table ?? (_table = CreateEmptyTableWithColumns());

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
      AddRange(Session.AllObjects<TEntity>());
      Table.Clear();
      AddRowsToTable();
    }

    protected abstract void AddRowsToTable();

    [NotNull]
    protected abstract DataTable CreateEmptyTableWithColumns();
  }
}