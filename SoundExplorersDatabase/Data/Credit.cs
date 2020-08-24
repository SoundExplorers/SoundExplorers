using System;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorersDatabase.Data {
  /// <summary>
  ///   Credit entity, specifying the Role played by an Artist in a Piece,
  ///   usually the instrument played by a musician. 
  /// </summary>
  public class Credit : EntityBase {
    private Artist _artist;
    private int _creditNo;
    private Role _role;
    public Credit() : base(typeof(Credit), nameof(CreditNo), typeof(Piece)) { }

    public Artist Artist {
      get => _artist;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Artist), value);
        _artist = value;
      }
    }

    public int CreditNo {
      get => _creditNo;
      set {
        if (value == 0) {
          throw new NoNullAllowedException("CreditNo '00' is not valid.");
        }
        UpdateNonIndexField();
        _creditNo = value;
        SimpleKey = value.ToString().PadLeft(2, '0');
      }
    }

    public Piece Piece {
      get => (Piece)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public Role Role {
      get => _role;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Role), value);
        _role = value;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      if (parentEntityType == typeof(Artist)) {
        _artist = (Artist)newParent;
      } else {
        _role = (Role)newParent;
      }
    }
  }
}