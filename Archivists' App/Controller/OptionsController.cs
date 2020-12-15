using System;
using JetBrains.Annotations;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the Options dialog box.
  /// </summary>
  [UsedImplicitly]
  public class OptionsController {
    private string _databaseFolderPath;

    /// <summary>
    ///   Initialises a new instance of the <see cref="OptionsController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    public OptionsController([NotNull] IView<OptionsController> view) {
      view.SetController(this);
    }

    public string DatabaseFolderPath {
      get => _databaseFolderPath ??
             Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      set => _databaseFolderPath = value;
    }
  }
}