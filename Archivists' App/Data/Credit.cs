using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Credit entity, specifying the Role played by an Artist in a Piece,
  ///   usually the instrument played by a musician.
  /// </summary>
  public class Credit : EntityBase {
    private Artist _artist = null!;
    private int _creditNo;
    private Role _role = null!;
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
        UpdateNonIndexField();
        _creditNo = value;
        SimpleKey = IntegerToSimpleKey(value, nameof(CreditNo));
      }
    }

    public Piece Piece {
      get => (Piece)IdentifyingParent!;
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
    protected override IEnumerable GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      if (parentEntityType == typeof(Artist)) {
        _artist = (newParent as Artist)!;
      } else {
        _role = (newParent as Role)!;
      }
    }
  }
}