using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Metadata for the columns of a binding list that links entities to a grid.
  /// </summary>
  /// <remarks>
  ///   A keyed list of entity columns with
  ///   <see cref="BindingColumn.Name" /> as the key.
  /// </remarks>
  public class BindingColumnList : List<BindingColumn> {
    /// <summary>
    ///   Returns the entity column with the specified name (case-insensitive).
    /// </summary>
    /// <param name="name">
    ///   The name of the column (case-insensitive).
    /// </param>
    /// <returns>
    ///   The entity column with the specified name (case-insensitive).
    /// </returns>
    public BindingColumn this[string name] =>
      FindColumn(name) ??
      throw new InvalidOperationException(
        $"In BindingColumnList.Item, cannot find '{name}' column.");

    /// <summary>
    ///   Add the specified entity column to the list,
    ///   provided its name is unique in the list.
    /// </summary>
    /// <param name="bindingColumn">
    ///   The entity column to be added.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The list already contains an entity column
    ///   of the same name.
    /// </exception>
    public new void Add(BindingColumn bindingColumn) {
      if (ContainsKey(bindingColumn.Name)) {
        throw new ArgumentException(
          $"The list already contains an entity column named {bindingColumn.Name}.",
          nameof(bindingColumn));
      }
      base.Add(bindingColumn);
    }

    /// <summary>
    ///   Returns whether the list contains
    ///   an entity column with the specified name.
    /// </summary>
    /// <param name="name">
    ///   The name of the column.
    /// </param>
    /// <returns>
    ///   Whether the list contains
    ///   an entity column with the specified name.
    /// </returns>
    public bool ContainsKey(string name) {
      return FindColumn(name) != null;
    }

    private BindingColumn? FindColumn(string name) {
      return (
        from BindingColumn bindingColumn in this
        where string.Compare(bindingColumn.Name, name,
          StringComparison.OrdinalIgnoreCase) == 0
        select bindingColumn).FirstOrDefault();
    }

    public int GetIndex(string propertyName) {
      return IndexOf(this[propertyName]);
    }
  } //End of class
} //End of namespace