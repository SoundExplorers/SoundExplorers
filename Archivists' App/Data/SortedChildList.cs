using System.Linq;
using VelocityDb.Collection.BTree;

namespace SoundExplorers.Data {
  /// <summary>
  ///   The SortedList that is the collection of children,
  ///   of the specified entity type, of a parent entity.
  /// </summary>
  /// <typeparam name="TChild">
  ///   The entity type of the child entities in the collection.
  /// </typeparam>
  public class SortedChildList<TChild> : BTreeMap<Key, TChild>
    where TChild : EntityBase {
    /// <summary>
    ///   Creates a SortedChildList instance.
    /// </summary>
    internal SortedChildList() : base(new KeyComparer(), null) { }

    public TChild this[int index] => this[Keys.ToList()[index]];
  }
}