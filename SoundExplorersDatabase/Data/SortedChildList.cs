using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class SortedChildList<TKey, TChild> : SortedList<TKey, TChild>
    where TChild : RelativeBase {
    internal SortedChildList([NotNull] RelativeBase parent) {
      Parent = parent;
    }

    private RelativeBase Parent { get; }

    public TChild this[int index] => Values[index];

    public bool Add(TChild child) {
      var childType = child.GetType();
      if (Parent.IsChangingChildrenOfType[childType]) {
        base.Add((TKey)child.Key, child);
        return true;
      }
      return Parent.AddChild(child);
    }

    public bool Remove(TChild child) {
      return Parent.RemoveChild(child);
    }

    [PublicAPI]
    public void Add(string notSupported, TChild doNotUse) {
      throw new NotSupportedException(
        "ParentChildren.Add(string, TChild) is not supported. Use ParentChildren.Add(TChild) instead.");
    }

    [PublicAPI]
    public bool Remove(string notSupported) {
      throw new NotSupportedException(
        "ParentChildren.Remove(string) is not supported. Use ParentChildren.Remove(TChild) instead.");
    }

    [PublicAPI]
    public new bool RemoveAt(int notSupported) {
      throw new NotSupportedException(
        "ParentChildren.RemoveAt(int) is not supported. Use ParentChildren.Remove(TChild) instead.");
    }
  }
}