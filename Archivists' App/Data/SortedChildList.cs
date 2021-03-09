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
  public class SortedChildList<TChild> : BTreeMap<Key, TChild>, ISortedChildList
    where TChild : EntityBase {
    internal SortedChildList() : base(new KeyComparer(), null) { }
    public TChild this[int index] => Values.ToList()[index];
    IEntity ISortedChildList.this[Key key] => this[key];

    void ISortedChildList.Add(Key key, IEntity child) {
      Add(key, (TChild)child);
    }

    void ISortedChildList.Remove(Key key) {
      Remove(key);
    }
  }
}