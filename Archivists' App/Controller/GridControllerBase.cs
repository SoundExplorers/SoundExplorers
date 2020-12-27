﻿using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Model;

namespace SoundExplorers.Controller {
  public abstract class GridControllerBase {
    protected GridControllerBase([NotNull] IEditorView editorView) {
      EditorView = editorView;
    }
    
    [CanBeNull] public IBindingList BindingList => List.BindingList;

    /// <summary>
    ///   Gets metadata about the database columns
    ///   represented by the Entity's field properties.
    /// </summary>
    [NotNull]
    internal BindingColumnList Columns => List.Columns;
    
    [NotNull] protected IEditorView EditorView { get; }

    /// <summary>
    ///   Gets the list of entities represented in the grid.
    /// </summary>
    protected abstract IEntityList List { get; }
  }
}