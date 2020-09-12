using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Data;
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
    /// <param name="tableName">
    ///   The name of the table that is to be initially selected.
    ///   An empty string for no table to be initially selected.
    /// </param>
    public SelectTableController([NotNull] IView<SelectTableController> view,
      [NotNull] string tableName) {
      EntityListTypeDictionary = CreateEntityListTypeDictionary();
      SelectedEntityListType = EntityListTypeDictionary[tableName];
      view.SetController(this);
    }

    [NotNull] public SortedDictionary<string, Type> EntityListTypeDictionary { get; }

    [NotNull] public Type SelectedEntityListType { get; set; }
    [NotNull] public string SelectedTableName { get; set; }

    /// <summary>
    ///   Creates a sorted dictionary of Entity or EntityList types,
    ///   with the Entity name as the key
    ///   and the type as the value.
    /// </summary>
    /// <returns>
    ///   The sorted dictionary created.
    /// </returns>
    [NotNull]
    private static SortedDictionary<string, Type> CreateEntityListTypeDictionary() {
      return new SortedDictionary<string, Type> {
        {string.Empty, null},
        {nameof(Event), typeof(EventList)},
        {nameof(Set), typeof(SetList)}
      };
    }
  }
}