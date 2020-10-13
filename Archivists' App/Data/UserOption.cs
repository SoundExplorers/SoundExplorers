using System;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity for persisting a user option for the application.
  /// </summary>
  public class UserOption : EntityBase {
    private string _optionName;
    private string _optionValue;
    private string _userId;
    public UserOption() : base(typeof(UserOption), nameof(SimpleKey), null) { }

    public string OptionName {
      get => _optionName;
      set {
        if (string.IsNullOrWhiteSpace(value)) {
          throw new NoNullAllowedException($"{nameof(OptionName)} may not be blank.");
        }
        UpdateNonIndexField();
        SetSimpleKey(UserId, value);
        _optionName = value;
      }
    }

    [CanBeNull]
    public string OptionValue {
      get => _optionValue;
      set {
        UpdateNonIndexField();
        _optionValue = value;
      }
    }

    public string UserId {
      get => _userId;
      set {
        if (string.IsNullOrWhiteSpace(value)) {
          throw new NoNullAllowedException($"{nameof(UserId)} may not be blank.");
        }
        UpdateNonIndexField();
        SetSimpleKey(value, OptionName);
        _userId = value;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(Type parentEntityType,
      EntityBase newParent) {
      throw new NotSupportedException();
    }

    private void SetSimpleKey([NotNull] string userId, [NotNull] string optionName) {
      SimpleKey = $"{userId}|{optionName}";
    }
  }
}