using System.Collections;

namespace SoundExplorers.Controller {
  public interface IGrid {
    void InvertCellColorScheme();
    
    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    /// <param name="list">
    ///   Optionally specifies the required list of entities that will populate the grid.
    ///   If null, the default, all entities of the class's entity type will be fetched
    ///   from the database.
    /// </param>
    void Populate(IList? list = null);
    
    void RestoreCellColorSchemeToDefault();
  }
}