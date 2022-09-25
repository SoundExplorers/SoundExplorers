using SoundExplorers.Model;

namespace SoundExplorers.Controller; 

public abstract class CellControllerBase {
  protected CellControllerBase(MainGridController mainGridController,
    string columnName) {
    MainGridController = mainGridController;
    Column = MainGridController.Columns[columnName];
  }

  internal BindingColumn Column { get; }
  protected MainGridController MainGridController { get; }
  public string TableName => MainGridController.TableName;
}