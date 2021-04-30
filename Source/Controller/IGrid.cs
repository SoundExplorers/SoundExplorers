﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Controller {
  public interface IGrid {
    object? CurrentCellValue { get; }
    int CurrentRowIndex { get; }
    IGridCellColorScheme CellColorScheme { get; }
    bool Enabled { get; set; }
    bool Focused { get; }

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    string Name { get; }

    int RowCount { get; }
    void BeginInvoke(Action action);
    void Focus();

    /// <summary>
    ///   Makes the specified row current, which will set focus and raise OnRowEnter. If
    ///   the new row index is specified, the insertion binding item will be added
    ///   (unless the new row is already current).
    /// </summary>
    void MakeRowCurrent(int rowIndex, bool async = false);

    /// <summary>
    ///   Populates and sorts the grid.
    /// </summary>
    void Populate();
  }
}