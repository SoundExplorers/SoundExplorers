using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ParentChildren : SortedList<string, Child> {
    internal ParentChildren(Parent parent) {
      Parent = parent;
    }

    private Parent Parent { get; }

    public Child this[int index] => Values[index];

    public bool Add(Child child) {
      return Parent.AddChild(child);
    }

    [PublicAPI]
    public new void Add(string notSupported, Child doNotUse) {
      throw new NotSupportedException(
        "ParentChildren.Add(string, Child) is not supported. Use ParentChildren.Add(Child) instead.");
    }

    public bool Remove(Child child) {
      return Parent.RemoveChild(child);
    }

    [PublicAPI]
    public new bool Remove(string notSupported) {
      throw new NotSupportedException(
        "ParentChildren.Remove(string) is not supported. Use ParentChildren.Remove(Child) instead.");
    }

    [PublicAPI]
    public new bool RemoveAt(int notSupported) {
      throw new NotSupportedException(
        "ParentChildren.RemoveAt(int) is not supported. Use ParentChildren.Remove(Child) instead.");
    }
  }
}