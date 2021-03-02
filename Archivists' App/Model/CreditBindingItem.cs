using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class CreditBindingItem : BindingItemBase<Credit, CreditBindingItem> {
    private string _creditNo = null!;
    private string? _artist;
    private string? _role;

    public string CreditNo {
      get => _creditNo;
      set {
        _creditNo = value;
        OnPropertyChanged(nameof(CreditNo));
      }
    }

    public string? Artist {
      get => _artist;
      set {
        _artist = value;
        OnPropertyChanged(nameof(Artist));
      }
    }

    public string? Role {
      get => _role;
      set {
        _role = value;
        OnPropertyChanged(nameof(Role));
      }
    }
    private Piece Piece { get; set; } = null!;

    protected override IDictionary<string, object?>
      CreateEntityPropertyValueDictionary() {
      Piece = (EntityList.IdentifyingParent as Piece)!;
      return new Dictionary<string, object?> {
        [nameof(CreditNo)] = CreditNo,
        [nameof(Artist)] = Artist,
        [nameof(Role)] = Role
      };
    }

    protected override void CopyValuesToEntityProperties(Credit credit) {
      // CreditNo must be set first, so that Artist.Credits, Role.Credits and
      // Piece.Credits will have the correct key for the Credit and therefore be in the
      // right sort order. And, to avoid VelocityDB exceptions (I am now not sure what),
      // Artist and Role must be set before Piece.
      credit.CreditNo = SimpleKeyToInteger(CreditNo, nameof(CreditNo));
      credit.Artist = (Artist)FindParent(Properties[nameof(Artist)])!;
      credit.Role = (Role)FindParent(Properties[nameof(Role)])!;
      credit.Piece = Piece;
    }

    protected override object? GetEntityPropertyValue(PropertyInfo property,
      PropertyInfo creditProperty) {
      switch (property.Name) {
        case nameof(CreditNo):
          string creditNoString = GetPropertyValue(nameof(CreditNo))!.ToString()!;
          return SimpleKeyToInteger(creditNoString, nameof(CreditNo));
        default:
          return base.GetEntityPropertyValue(property, creditProperty);
      }
    }
    
    protected override string GetSimpleKey() {
      // Validate and format the CreditNo, which may have been entered by the user.
      return EntityBase.IntegerToSimpleKey(SimpleKeyToInteger(
          CreditNo, nameof(CreditNo)),
        nameof(CreditNo));
    }

    private void ValidateArtistOnInsertion() {
      if (string.IsNullOrWhiteSpace(Artist)) {
        throw EntityBase.CreateParentNotSpecifiedException(
          nameof(Credit), Key, nameof(Artist));
      }
    }

    private void ValidateRoleOnInsertion() {
      if (string.IsNullOrWhiteSpace(Role)) {
        throw EntityBase.CreateParentNotSpecifiedException(
          nameof(Credit), Key, nameof(Role));
      }
    }

    internal override void ValidateInsertion() {
      base.ValidateInsertion();
      ValidateArtistOnInsertion();
      ValidateRoleOnInsertion();
    }
  }
}