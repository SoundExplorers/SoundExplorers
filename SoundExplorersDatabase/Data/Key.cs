using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Key : IKey {
    private readonly RelativeBase _identifyingParent;
    private readonly string _simpleKey;

    internal Key([NotNull] Func<string> getSimpleKey,
      [NotNull] Func<RelativeBase> getIdentifyingParent) {
      GetSimpleKey = getSimpleKey ??
                     throw new ArgumentNullException(nameof(getSimpleKey));
      GetIdentifyingParent = getIdentifyingParent ??
                             throw new ArgumentNullException(
                               nameof(getIdentifyingParent));
    }

    internal Key([CanBeNull] string simpleKey,
      [CanBeNull] RelativeBase identifyingParent) {
      _simpleKey = simpleKey;
      _identifyingParent = identifyingParent;
    }

    [CanBeNull] private Func<RelativeBase> GetIdentifyingParent { get; }
    [CanBeNull] private Func<string> GetSimpleKey { get; }

    [CanBeNull]
    public RelativeBase IdentifyingParent {
      get {
        if (GetIdentifyingParent != null) {
          return GetIdentifyingParent.Invoke();
        }
        return _identifyingParent;
      }
    }

    [CanBeNull]
    public string SimpleKey {
      get {
        if (GetSimpleKey != null) {
          return GetSimpleKey.Invoke();
        }
        return _simpleKey;
      }
    }

    public bool Matches(IKey otherKey) {
      bool result;
      if (otherKey != null) {
        if (SimpleKey == otherKey.SimpleKey) {
          if (IdentifyingParent == null &&
              otherKey.IdentifyingParent == null) {
            result = true;
          } else if (IdentifyingParent != null &&
                     IdentifyingParent.Equals(otherKey.IdentifyingParent)) {
            result = true;
          } else {
            result = false;
          }
        } else {
          result = false;
        }
      } else {
        result = false;
      }
      return result;
    }

    public override string ToString() {
      if (SimpleKey != null) {
        return IdentifyingParent != null
          ? $"{SimpleKey} {IdentifyingParent?.Key}"
          : SimpleKey;
      }
      return string.Empty;
    }
  }
}