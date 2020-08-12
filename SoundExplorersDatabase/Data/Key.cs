using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Key {
    private readonly IRelative _identifyingParent;
    private readonly string _simpleKey;

    internal Key([NotNull] Func<string> getSimpleKey,
      [NotNull] Func<IRelative> getIdentifyingParent) {
      GetSimpleKey = getSimpleKey ??
                     throw new ArgumentNullException(nameof(getSimpleKey));
      GetIdentifyingParent = getIdentifyingParent ??
                             throw new ArgumentNullException(
                               nameof(getIdentifyingParent));
    }

    internal Key([CanBeNull] string simpleKey,
      [CanBeNull] IRelative identifyingParent) {
      _simpleKey = simpleKey;
      _identifyingParent = identifyingParent;
    }

    [CanBeNull] private Func<IRelative> GetIdentifyingParent { get; }
    [CanBeNull] private Func<string> GetSimpleKey { get; }

    [CanBeNull]
    public IRelative IdentifyingParent => GetIdentifyingParent != null
      ? GetIdentifyingParent.Invoke()
      : _identifyingParent;

    [CanBeNull]
    public string SimpleKey =>
      GetSimpleKey != null ? GetSimpleKey.Invoke() : _simpleKey;

    public override string ToString() {
      if (SimpleKey != null) {
        return IdentifyingParent != null
          ? $"{SimpleKey} {IdentifyingParent?.Key}"
          : SimpleKey;
      }
      return string.Empty;
    }

    public static bool operator ==([CanBeNull] Key key1, [CanBeNull] Key key2) {
      if ((object)key1 != null) {
        return key1.Equals(key2);
      }
      return (object)key2 == null;
    }

    public static bool operator !=([CanBeNull] Key key1, [CanBeNull] Key key2) {
      return !(key1 == key2);
    }

    public static bool operator <([CanBeNull] Key key1, [CanBeNull] Key key2) {
      if (key1 == key2) {
        return false;
      }
      if (key1 == null) {
        return true;
      }
      if (key2 == null) {
        return false;
      }
      if (key1.IdentifyingParent != null && key2.IdentifyingParent != null) {
        if (key1.IdentifyingParent.Key < key2.IdentifyingParent.Key) {
          return true;
        }
        if (key1.IdentifyingParent.Key > key2.IdentifyingParent.Key) {
          return false;
        }
        return string.CompareOrdinal(key1.SimpleKey, key2.SimpleKey) < 0;
      }
      if (key1.IdentifyingParent == null && key2.IdentifyingParent == null) {
        return string.CompareOrdinal(key1.SimpleKey, key2.SimpleKey) < 0;
      }
      return key1.IdentifyingParent == null && key2.IdentifyingParent != null;
    }

    public static bool operator >([CanBeNull] Key key1, [CanBeNull] Key key2) {
      return !(key1 < key2) && key1 != key2;
    }

    public override bool Equals(object obj) {
      var keyToMatch = obj as Key;
      if (keyToMatch == null) {
        return false;
      }
      if (IdentifyingParent != null && keyToMatch.IdentifyingParent != null) {
        if (IdentifyingParent.Key.Equals(keyToMatch.IdentifyingParent.Key)) {
          return string.CompareOrdinal(SimpleKey, keyToMatch.SimpleKey) == 0;
        }
        return false;
      }
      if (IdentifyingParent == null && keyToMatch.IdentifyingParent == null) {
        return string.CompareOrdinal(SimpleKey, keyToMatch.SimpleKey) == 0;
      }
      return false;
    }

    public override int GetHashCode() {
      int hashCode1 = SimpleKey != null ? SimpleKey.GetHashCode() : 0;
      int hashCode2 = IdentifyingParent != null
        ? IdentifyingParent.Key.GetHashCode()
        : 0;
      return hashCode1 + hashCode2;
    }
  }
}