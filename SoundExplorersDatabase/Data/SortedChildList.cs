using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class SortedChildList<TChild> : SortedList<Key, TChild>, IDictionary
    where TChild : RelativeBase {
    internal SortedChildList([NotNull] RelativeBase parent) : base(
      new KeyComparer()) {
      Parent = parent ??
               throw new ArgumentNullException(nameof(parent));
    }

    private RelativeBase Parent { get; }
    public TChild this[int index] => Values[index];

    object IDictionary.this[object key] {
      get => this[ToKey(key)];
      set => this[ToKey(key)] = ToChild(value);
    }

    void IDictionary.Add(object key, object value) {
      base.Add(ToKey(key), ToChild(value));
    }

    bool IDictionary.Contains(object key) {
      var keyToMatch = ToKey(key);
      return (
        from Key foundKey in Keys
        where foundKey == keyToMatch
        select foundKey).Any();
    }

    public void Add([NotNull] TChild child) {
      Parent.AddChild(child);
    }

    public void Remove([NotNull] TChild child) {
      Parent.RemoveChild(child, false);
    }

    [UsedImplicitly]
    public new void Add(Key notSupported, TChild doNotUse) {
      throw new NotSupportedException(
        "ParentChildren.Add(Key, TChild) is not supported. " +
        "Use ParentChildren.Add(TChild) instead.");
    }

    private static TChild ToChild(object value) {
      var child = value as TChild ??
                  throw new ArgumentException(
                    $"The specified value is not of type {typeof(TChild).Name}",
                    nameof(value));
      return child;
    }

    private static Key ToKey(object key) {
      var keyToMatch = key as Key ??
                       throw new ArgumentException(
                         "The specified key is not of type Key", nameof(key));
      return keyToMatch;
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