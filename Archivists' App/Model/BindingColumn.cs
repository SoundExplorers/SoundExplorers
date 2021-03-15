using System;
using SoundExplorers.Common;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Column data for a binding list that links entities to a grid.
  /// </summary>
  public class BindingColumn : IBindingColumn {
    private string? _displayName;
    private SessionBase? _session;

    internal BindingColumn(string propertyName, Type valueType,
      ReferenceType? referenceType = null) {
      PropertyName = propertyName;
      ValueType = valueType;
      if (referenceType != null) {
        ReferencedEntityListType = referenceType.ReferencedEntityListType;
        ReferencedPropertyName = referenceType.ReferencedPropertyName;
      }
    }

    public bool IsInKey { get; internal init; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database. The setter
    ///   should only be needed for testing.
    /// </summary>
    internal SessionBase Session {
      get => _session ??= Global.Session;
      set => _session = value;
    }

    public ReferenceableItemList? ReferenceableItems { get; internal set; }

    /// <summary>
    ///   Gets the type of the referenced entity list. Null if the column does not
    ///   reference a column on another entity list.
    /// </summary>
    public Type? ReferencedEntityListType { get; }

    /// <summary>
    ///   Gets the name of the corresponding property of the referenced entity. Null if
    ///   the column does not reference a another entity type.
    /// </summary>
    public string? ReferencedPropertyName { get; }

    /// <summary>
    ///   Gets the name of the referenced entity list's main table.
    ///   Null if the column does not reference a column on another entity list.
    /// </summary>
    public string? ReferencedTableName =>
      ReferencedEntityListType?.Name.Replace("List", string.Empty);

    /// <summary>
    ///   Gets or sets the display name to be used in the grid column header.
    ///   Defaults to <see cref="PropertyName" />.
    /// </summary>
    public string DisplayName {
      get => _displayName ??= PropertyName;
      internal init => _displayName = value;
    }

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
    public int EditWidth { get; set; }

    /// <summary>
    ///   Gets whether the column is to be visible on the grid. If false, it needs to be
    ///   hidden. Default: true.
    /// </summary>
    public bool IsVisible { get; internal init; } = true;

    /// <summary>
    ///   Gets the name of the BindingItem property to be bound to the column in the
    ///   grid.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///   Gets whether the column references another entity.
    /// </summary>
    public bool ReferencesAnotherEntity =>
      !string.IsNullOrWhiteSpace(ReferencedPropertyName);

    /// <summary>
    ///   Gets the type of the values that are to be shown for the column on the grid.
    /// </summary>
    /// <remarks>
    ///   This can indicate any special cell editor control that may be required on the
    ///   main grid for the value type, such as a DateTimePicker for a DateTime column or
    ///   a CheckBox for a boolean column. Or it can indicate required type-specific
    ///   validation, as for as integer column.
    /// </remarks>
    public Type ValueType { get; }

    internal void FetchReferenceableItems() {
      ReferenceableItems = new ReferenceableItemList(this);
      bool isTransactionRequired = !Session.InTransaction;
      if (isTransactionRequired) {
        Session.BeginRead();
      }
      ReferenceableItems.Fetch();
      if (isTransactionRequired) {
        Session.Commit();
      }
    }
  } //End of class
} //End of namespace