using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  /// <summary>
  ///   Controller for the Select a Table Editor dialog box.
  /// </summary>
  [UsedImplicitly]
  public class SelectEditorController {
    /// <summary>
    ///   Initialises a new instance of the <see cref="EditorController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    /// <param name="initiallySelectedTableName">
    ///   The name of the table that is to be initially selected. If an empty string or
    ///   an unsupported table name, no table to be initially selected.
    /// </param>
    public SelectEditorController(IView<SelectEditorController> view,
      string initiallySelectedTableName) {
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

    public SortedDictionary<string, Type> EntityListTypeDictionary { get; }
    public Type? SelectedEntityListType { get; set; }
    public string SelectedTableName { get; set; }
  }
}