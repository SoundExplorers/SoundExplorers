using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Controller {
  public interface IGrid {
    IGridCellColorScheme CellColorScheme { get; }
    bool Focused { get; }
    
    /// <summary>
    ///   The grid's name. Useful for debugging. 
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")] 
    string Name { get; }
    
    int RowCount { get; }
    void Focus();
    
    /// <summary>
    ///   Makes the specified row current, which will set focus and raise OnRowEnter.
    /// </summary>
    void MakeRowCurrent(int rowIndex, bool async = false);
    void OnPopulated();

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