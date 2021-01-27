using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   The SortedList that is the collection of children,
  ///   of the specified entity type, of a parent entity.
  /// </summary>
  /// <typeparam name="TChild">
  ///   The entity type of the child entities in the collection.
  /// </typeparam>
  public class SortedChildList<TChild> : SortedList<Key, TChild>
    where TChild : EntityBase {
    /// <summary>
    ///   Creates a SortedChildList instance.
    /// </summary>
    internal SortedChildList() : base(new KeyComparer()) { }

    public TChild this[int index] => Values[index];

    public TChild this[[CanBeNull] string simpleKey,
      EntityBase? identifyingParent = null] =>
      this[new Key(simpleKey, identifyingParent)];

    [UsedImplicitly]
    public new void Add(Key notSupported, TChild doNotUse) {
      throw new NotSupportedException();
    }

    [UsedImplicitly]
    public new bool Remove(Key notSupported) {
      throw new NotSupportedException();
    }

    [UsedImplicitly]
    public new bool RemoveAt(int notSupported) {
      throw new NotSupportedException();
    }
  }
}