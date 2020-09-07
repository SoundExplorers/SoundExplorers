using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   The key derived from an entity's SimpleKey and, where applicable,
  ///   IdentifyingParent properties, for use as the entity's key
  ///   in any SortedChildLists of which it is a member.
  /// </summary>
  public class Key {
    private readonly IEntity _identifyingParent;
    private readonly string _simpleKey;

    /// <summary>
    ///   Use this constructor to instantiate a key that
    ///   is the property of an entity and that will be
    ///   the entity's key in any in SortedLists
    ///   of which the entity is a member.
    ///   May also be used to instantiate a key for comparison
    ///   with a key that is the property of an entity
    /// </summary>
    /// <param name="owner">
    ///   The entity whose Key property is set to this key.
    /// </param>
    /// <param name="identifyingParent">
    ///   For comparative use only, optionally specifies
    ///   the identifying parent entity.
    /// </param>
    internal Key([NotNull] IEntity owner,
      IEntity identifyingParent = null) {
      Owner = owner;
      _simpleKey = Owner.SimpleKey;
      _identifyingParent = identifyingParent;
    }

    /// <summary>
    ///   Use this constructor for a Key that can be used for
    ///   comparisons with Keys that are properties of entities.
    /// </summary>
    /// <param name="simpleKey">
    ///   The simple key to be used for comparison.
    /// </param>
    /// <param name="identifyingParent">
    ///   The identifying parent entity, if applicable,
    ///   to be used for comparison.
    /// </param>
    public Key([CanBeNull] string simpleKey,
      [CanBeNull] IEntity identifyingParent) {
      _simpleKey = simpleKey;
      _identifyingParent = identifyingParent;
    }

    [CanBeNull]
    private IEntity IdentifyingParent =>
      Owner?.IdentifyingParent ?? _identifyingParent;

    [CanBeNull] private IEntity Owner { get; }
    [CanBeNull] private string SimpleKey => Owner?.SimpleKey ?? _simpleKey;

    public override bool Equals(object obj) {
      var keyToMatch = obj as Key;
      if ((object)keyToMatch == null) {
        return false;
      }
      if (IdentifyingParent != null && keyToMatch.IdentifyingParent != null) {
        if (IdentifyingParent.Key.Equals(keyToMatch.IdentifyingParent.Key)) {
          return SimpleKey == keyToMatch.SimpleKey;
        }
        return false;
      }
      if (IdentifyingParent == null && keyToMatch.IdentifyingParent == null) {
        return SimpleKey == keyToMatch.SimpleKey;
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

    public static bool operator ==([CanBeNull] Key key1, [CanBeNull] Key key2) {
      if ((object)key1 != null) {
        return key1.Equals(key2);
      }
      return (object)key2 == null;
    }

    public static bool operator >([CanBeNull] Key key1, [CanBeNull] Key key2) {
      return !(key1 < key2) && key1 != key2;
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

    public override string ToString() {
      if (SimpleKey != null) {
        return IdentifyingParent != null
          ? $"{SimpleKey} | {IdentifyingParent?.Key}"
          : SimpleKey;
      }
      return string.Empty;
    }
  }
}