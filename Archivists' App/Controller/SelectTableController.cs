using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the Select a Table dialog box.
  /// </summary>
  [UsedImplicitly]
  public class SelectTableController {
    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    /// <param name="initiallySelectedTableName">
    ///   The name of the table that is to be initially selected.
    ///   If an empty string or an unsupported table name,
    ///   for no table to be initially selected.
    /// </param>
    public SelectTableController([NotNull] IView<SelectTableController> view,
      [NotNull] string initiallySelectedTableName) {
      EntityListTypeDictionary = Global.CreateEntityListTypeDictionary();
      if (EntityListTypeDictionary.ContainsKey(initiallySelectedTableName)) {
        SelectedEntityListType = EntityListTypeDictionary[initiallySelectedTableName];
        SelectedTableName = initiallySelectedTableName;
      } else {
        SelectedEntityListType = null;
        SelectedTableName = string.Empty;
      }
      view.SetController(this);
    }

    [NotNull] public SortedDictionary<string, Type> EntityListTypeDictionary { get; }
    [CanBeNull] public Type SelectedEntityListType { get; set; }
    [NotNull] public string SelectedTableName { get; set; }
  }
}