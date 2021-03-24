using System;

namespace SoundExplorers.Data {
  /// <summary>
  ///   The key derived from an entity's SimpleKey and, where applicable,
  ///   IdentifyingParent properties, for use as the entity's key
  ///   in any SortedChildLists of which it is a member.
  /// </summary>
  /// <remarks>
  ///   SimpleKey has priority over IdentifyingParent when keys are compared.
  /// </remarks>
  public class Key {
    private readonly IEntity? _identifyingParent;
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
    ///   For entity types that have an identifying parent, specifies the identifying
    ///   parent entity. Otherwise null, the default. This is for comparative use only,
    ///   as only the owner's simple key is relevant its order in the sorted child list.
    /// </param>
    internal Key(IEntity owner,
      IEntity? identifyingParent = null) {
      Owner = owner;
      _simpleKey = Owner.SimpleKey;
      _identifyingParent = identifyingParent;
    }

    /// <summary>
    ///   Use this constructor for a Key that can be used for comparisons with Keys that
    ///   are properties of entities.
    /// </summary>
    /// <param name="simpleKey">
    ///   The simple key to be used for comparison.
    /// </param>
    /// <param name="identifyingParent">
    ///   The identifying parent entity, if applicable, to be used for comparison.
    /// </param>
    public Key(string simpleKey,
      IEntity? identifyingParent) {
      _simpleKey = simpleKey;
      _identifyingParent = identifyingParent;
    }

    private IEntity? IdentifyingParent =>
      Owner?.IdentifyingParent ?? _identifyingParent;

    private IEntity? Owner { get; }
    private string SimpleKey => Owner?.SimpleKey ?? _simpleKey;

    public static int CompareSimpleKeys(string? simpleKey1, string? simpleKey2) {
      return string.Compare(simpleKey1, simpleKey2,
        StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) {
      var keyToMatch = obj as Key;
      // We have to cast the key to a nullable object for the == comparison. Otherwise
      // the == operator will loop.
      if ((object?)keyToMatch == null) {
        return false;
      }
      if (IdentifyingParent != null && keyToMatch.IdentifyingParent != null) {
        if (IdentifyingParent.Key.Equals(keyToMatch.IdentifyingParent.Key)) {
          return CompareSimpleKeys(SimpleKey, keyToMatch.SimpleKey) == 0;
        }
        return false;
      }
      if (IdentifyingParent == null && keyToMatch.IdentifyingParent == null) {
        return CompareSimpleKeys(SimpleKey, keyToMatch.SimpleKey) == 0;
      }
      return false;
    }

    public override int GetHashCode() {
      return ToString().GetHashCode();
      // int hashCode1 = SimpleKey.GetHashCode();
      // int hashCode2 = IdentifyingParent != null
      //   ? IdentifyingParent.Key.GetHashCode()
      //   : 0;
      // return hashCode1 + hashCode2;
    }

    public static bool operator ==(Key? key1, Key? key2) {
      // We have to cast the keys to nullable objects for the == and != comparisons.
      // Otherwise this == operator will loop.
      if ((object?)key1 != null) {
        return key1.Equals(key2);
      }
      return (object?)key2 == null;
    }

    public static bool operator >(Key? key1, Key? key2) {
      return !(key1 < key2) && key1 != key2;
    }

    public static bool operator !=(Key? key1, Key? key2) {
      return !(key1 == key2);
    }

    public static bool operator <(Key? key1, Key? key2) {
      if (key1 == key2) {
        return false;
      }
      if (key1 == null) {
        return true;
      }
      if (key2 == null) {
        return false;
      }
      // Order by simple key first
      switch (CompareSimpleKeys(key1.SimpleKey, key2.SimpleKey)) {
        case < 0:
          return true;
        case > 0:
          return false;
      }
      // Simple keys are equal. So compare identifying parents.
      if (key1.IdentifyingParent != null && key2.IdentifyingParent != null) {
        return key1.IdentifyingParent.Key < key2.IdentifyingParent.Key;
      }
      return key1.IdentifyingParent == null && key2.IdentifyingParent != null;
    }

    public override string ToString() {
      return IdentifyingParent != null
        ? $"{SimpleKey} | {IdentifyingParent?.Key}"
        : SimpleKey;
    }
  }
}