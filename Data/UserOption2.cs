using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity for persisting a user option for the application.
  /// </summary>
  public class UserOption2 : EntityBase2 {
    private string _optionName = null!;
    private string _optionValue = null!;
    private string _userId = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public UserOption2() : base(typeof(UserOption2), nameof(SimpleKey),
      null) { }

    public string OptionName {
      get => _optionName;
      set {
        if (string.IsNullOrWhiteSpace(value)) {
          throw new PropertyConstraintException($"{nameof(OptionName)} may not be blank.",
            nameof(OptionName));
        }
        Update();
        SetSimpleKey(UserId, value);
        _optionName = value;
      }
    }

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
          throw new PropertyConstraintException($"{nameof(UserId)} may not be blank.",
            nameof(UserId));
        }
        Update();
        SetSimpleKey(value, OptionName);
        _userId = value;
      }
    }

    private void SetSimpleKey(string userId, string optionName) {
      SimpleKey = $"{userId}|{optionName}";
    }
  }
}