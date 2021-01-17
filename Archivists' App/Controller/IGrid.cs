using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Controller {
  public interface IGrid {
    // int CurrentRowIndex { get; }
    bool Focused { get; }
    
    /// <summary>
    ///   The grid's name. Useful for debugging. 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")] 
    string Name { get; }
    
    int RowCount { get; }
    void Focus();
    void MakeRowCurrent(int rowIndex);

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities that will populate the grid.
    ///   If null, the default, all entities of the class's entity type will be fetched
    ///   from the database.
    /// </param>
    void Populate(IList? list = null);
  }
}