using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the Options dialog box.
  /// </summary>
  [UsedImplicitly]
  public class OptionsController {
    /// <summary>
    ///   Initialises a new instance of the <see cref="OptionsController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    public OptionsController(IView<OptionsController> view) {
      view.SetController(this);
    }

    public string? DatabaseFolderPath { get; private set; }
    public string? Message { get; private set; }

    public void LoadDatabaseConfig() {
      var config = CreateDatabaseConfig();
      try {
        config.Load();
        DatabaseFolderPath = config.DatabaseFolderPath;
        Message = "To change the database folder path, " +
                  "please edit database configuration file\r\n" +
                  $"'{config.ConfigFilePath}'\r\n" +
                  $"and then restart {GetProductName()}.";
      } catch (ApplicationException exception) {
        DatabaseFolderPath = string.Empty;
        Message = exception.Message +
                  $"\r\nPlease fix. Then restart {GetProductName()}.";
      }
    }

    [ExcludeFromCodeCoverage]
    protected virtual IDatabaseConfig CreateDatabaseConfig() {
      return new DatabaseConfig();
    }

    [ExcludeFromCodeCoverage]
    protected virtual string GetProductName() {
      return Global.GetProductName();
    }
  }
}