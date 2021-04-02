﻿using System;

namespace SoundExplorers.Common {
  public interface IBindingColumn {
    /// <summary>
    ///   Gets the display name to be used in the grid column header.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///   Gets or sets the number of pixels to which the grid column's width should be
    ///   temporarily expanded (but not contracted) when an edit of one one of the
    ///   column's cells begins. -1 if the column's width does not need to be expanded
    ///   when one of its cells is edited.
    /// </summary>
    /// <remarks>
    ///   When the cell edit ends, the column's width will be auto-fitted to the column's
    ///   contents.
    /// </remarks>
    int EditWidth { get; set; }

    /// <summary>
    ///   Gets whether the column is to be visible on the grid. If false, it needs to be
    ///   hidden. Default: true.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    ///   Gets the name of the BindingItem property to be bound to the column in the
    ///   grid.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    ///   Gets whether the column references another entity.
    /// </summary>
    bool ReferencesAnotherEntity { get; }

    /// <summary>
    ///   Gets the type of the values that are to be shown for the column on the grid.
    /// </summary>
    Type ValueType { get; }
  }
}