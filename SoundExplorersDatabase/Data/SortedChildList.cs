using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  
  /// <summary>
  /// The SortedList that is the collection of children,
  /// of the specified entity type, of a parent entity.
  /// </summary>
  /// <typeparam name="TChild">
  /// The entity type of the child entities in the collection.
  /// </typeparam>
  public class SortedChildList<TChild> : SortedList<Key, TChild>
    where TChild : EntityBase {
    
    /// <summary>
    /// Creates a SortedChildList instance. 
    /// </summary>
    /// <param name="parent">
    /// The parent entity that owns the collection of children.
    /// </param>
    internal SortedChildList([NotNull] EntityBase parent) : base(
      new KeyComparer()) {
      Parent = parent ??
               throw new ArgumentNullException(nameof(parent));
    }

    public TChild this[int index] => Values[index];

    public TChild this[[CanBeNull] string simpleKey,
      EntityBase identifyingParent = null] =>
      this[new Key(simpleKey, identifyingParent)];

    private EntityBase Parent { get; }

    public void Add([NotNull] TChild child) {
      Parent.AddChild(child);
    }

    [UsedImplicitly]
    public new void Add(Key notSupported, TChild doNotUse) {
      throw new NotSupportedException(
        "ParentChildren.Add(Key, TChild) is not supported. " +
        "Use ParentChildren.Add(TChild) instead.");
    }

    public void Remove([NotNull] TChild child) {
      Parent.RemoveChild(child, false);
    }

    [UsedImplicitly]
    public new bool Remove(Key notSupported) {
      throw new NotSupportedException(
        "ParentChildren.Remove(string) is not supported. Use ParentChildren.Remove(TChild) instead.");
    }

    [UsedImplicitly]
    public new bool RemoveAt(int notSupported) {
      throw new NotSupportedException(
        "ParentChildren.RemoveAt(int) is not supported. Use ParentChildren.Remove(TChild) instead.");
    }
  }
}