using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Common;
using SoundExplorers.Data;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the Select a Table dialog box.
  /// </summary>
  [UsedImplicitly]
  public class SelectTableController {
    private SortedDictionary<string, Type> _entityListTypes;
    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    /// <param name="tableName">
    ///   The name of the table that is to be initially selected.
    ///   An empty string for no table to be initially selected.
    /// </param>
    public SelectTableController([NotNull] IView<SelectTableController> view,
      [NotNull] string tableName) {
      view.SetController(this);
      TableName = tableName;
    }
    
    [NotNull] public string TableName { get; set; }

    [NotNull] public SortedDictionary<string, Type> EntityListTypes {
      get => _entityListTypes ?? (_entityListTypes = Factory<IEntityList>.Types);
      // The setter is for testing.
      // ReSharper disable once UnusedMember.Global
      set => _entityListTypes = value;
    }
  }
}