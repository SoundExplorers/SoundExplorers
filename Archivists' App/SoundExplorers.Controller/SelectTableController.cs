﻿using System;
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

    /// <summary>
    ///   Initialises a new instance of the <see cref="TableController" /> class.
    /// </summary>
    /// <param name="view">
    ///   The view to be shown.
    /// </param>
    /// <param name="tableName">
    ///   The name of the table that is to be initially selected.
    ///   An emtpy string for no table to be initially selected.
    /// </param>
    public SelectTableController([NotNull] IView<SelectTableController> view,
      [NotNull] string tableName) {
      view.SetController(this);
      TableName = tableName;
    }
    
    [NotNull] public string TableName { get; set; }

    [NotNull]
    public SortedDictionary<string, Type> EntityListTypes => Factory<IEntityList>.Types;
  }
}