using System;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Column data for a binding list that links entities to a grid.
  /// </summary>
  public class BindingColumn {
    private ReferenceableItemList? _referenceableItems;
    private SessionBase? _session;

    public BindingColumn(string name,
      Type? referencedEntityListType = null, string? referencedPropertyName = null) {
      Name = name ?? throw new ArgumentNullException(nameof(name));
      if (referencedEntityListType != null &&
          !referencedEntityListType.GetInterfaces().Contains(typeof(IEntityList))) {
        throw new ArgumentException(
          $"The Type specified by {nameof(referencedEntityListType)} " +
          $"'{referencedEntityListType}' is invalid. If specified, it must " +
          $"implement the {nameof(IEntityList)} interface.",
          nameof(referencedEntityListType));
      }
      if (referencedEntityListType != null && referencedPropertyName == null ||
          referencedEntityListType == null && referencedPropertyName != null) {
        throw new InvalidOperationException(
          $"Both or neither of {nameof(referencedEntityListType)} and "
          + $"{nameof(referencedPropertyName)} must be specified.");
      }
      ReferencedEntityListType = referencedEntityListType;
      ReferencedPropertyName = referencedPropertyName;
    }

    /// <summary>
    ///   Gets the display name to be used for reporting.
    ///   Defaults to <see cref="Name" />.
    /// </summary>
    public string? DisplayName { get; internal set; }

    public bool IsInKey { get; internal set; }

    /// <summary>
    ///   Gets the column's property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///   Gets or sets the session to be used for accessing the database.
    ///   The setter should only be needed for testing.
    /// </summary>
    internal SessionBase Session {
      get => _session ??= Global.Session;
      set => _session = value;
    }

    public ReferenceableItemList ReferenceableItems =>
      _referenceableItems ??= FetchReferenceableItems();

    /// <summary>
    ///   Gets the type of the referenced entity list.
    ///   Null if the column does not reference a column on another entity list.
    /// </summary>
    public Type? ReferencedEntityListType { get; }

    /// <summary>
    ///   Gets the name of the corresponding property of the referenced entity.
    ///   Null if the column does not reference a another entity type.
    /// </summary>
    public string? ReferencedPropertyName { get; }

    /// <summary>
    ///   Gets the name of the referenced entity list's main table.
    ///   Null if the column does not reference a column on another entity list.
    /// </summary>
    public string? ReferencedTableName =>
      ReferencedEntityListType?.Name.Replace("List", string.Empty);

    private ReferenceableItemList FetchReferenceableItems() {
      var result = new ReferenceableItemList(this);
      bool isTransactionRequired = !Session.InTransaction;
      if (isTransactionRequired) {
        Session.BeginRead();
      }
      result.Fetch();
      if (isTransactionRequired) {
        Session.Commit();
      }
      return result;
    }
  } //End of class
} //End of namespace