using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class SortedChildList<TKey, TChild> : SortedList<TKey, TChild>
    where TChild : RelativeBase {
    internal SortedChildList([NotNull] RelativeBase parent) {
      Parent = parent ??
               throw new ArgumentNullException(nameof(parent));
    }

    private RelativeBase Parent { get; }
    public TChild this[int index] => Values[index];

    public void Add([NotNull] TChild child) {
      Parent.AddChild(child);
    }

    public void Remove([NotNull] TChild child) {
      Parent.RemoveChild(child, false);
    }

    [UsedImplicitly]
    public void Add(string notSupported, TChild doNotUse) {
      throw new NotSupportedException(
        "ParentChildren.Add(string, TChild) is not supported. " +
        "Use ParentChildren.Add(TChild) instead.");
    }

    [UsedImplicitly]
    public bool Remove(string notSupported) {
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