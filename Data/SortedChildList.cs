﻿using System.Linq;
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

    /// <summary>
    ///   From VelocityDB User's Guide:
    ///   'It is recommended that you make the following override in your
    ///   OptimizedPersistable subclass for better performance. ...
    ///   We may make this default but it could break existing code
    ///   so it is not a trivial change.'
    /// </summary>
    public override bool AllowOtherTypesOnSamePage => false;

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