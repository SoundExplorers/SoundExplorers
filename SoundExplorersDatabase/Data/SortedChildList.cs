using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class SortedChildList<TChild> : SortedList<Key, TChild>
    where TChild : RelativeBase {
    internal SortedChildList([NotNull] RelativeBase parent) : base(
      new KeyComparer()) {
      Parent = parent ??
               throw new ArgumentNullException(nameof(parent));
    }

    public TChild this[int index] => Values[index];

    private RelativeBase Parent { get; }

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