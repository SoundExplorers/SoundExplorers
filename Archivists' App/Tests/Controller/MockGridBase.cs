﻿using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Controller;

namespace SoundExplorers.Tests.Controller {
  public abstract class MockGridBase : IGrid {
    private bool _enabled;

    protected MockGridBase() {
      CellColorScheme = new MockGridCellColorScheme();
    }

    internal MockGridCellColorScheme CellColorScheme { get; }

    public bool Enabled {
      get => _enabled;
      set {
        _enabled = value;
        if (value) {
          EnableCount++;
        } else {
          DisableCount++;
        }
      }
    }

    internal int DisableCount { get; private set; }
    internal int EnableCount { get; private set; }
    internal int MakeRowCurrentCount { get; private set; }
    protected GridControllerBase Controller { get; set; } = null!;
    public int CurrentRowIndex { get; protected set; }
    IGridCellColorScheme IGrid.CellColorScheme => CellColorScheme;
    public bool Focused { get; internal set; }

    /// <summary>
    ///   The grid's name. Useful for debugging.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [ExcludeFromCodeCoverage]
    public abstract string Name { get; }

    public abstract int RowCount { get; }

    public void MakeRowCurrent(int rowIndex, bool async = false) {
      //Debug.WriteLine($"MockGridBase.MakeRowCurrent: row {rowIndex}");
      MakeRowCurrentCount++;
      CurrentRowIndex = rowIndex;
      Controller.OnRowEnter(rowIndex);
    }

    public void OnPopulated() {
      Controller.OnPopulatedAsync();
    }

    public void Populate(IList? list = null) {
      Controller.Populate(list);
    }

    public virtual void Focus() {
      Focused = true;
      Controller.PrepareForFocus();
      Controller.OnGotFocus();
    }

    internal void SetCurrentRowIndex(int rowIndex) {
      CurrentRowIndex = rowIndex;
    }
  }
}