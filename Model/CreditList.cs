using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class CreditList : EntityListBase<Credit, CreditBindingItem> {
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public CreditList() : base(typeof(PieceList)) { }

    protected override CreditBindingItem CreateBindingItem(Credit credit) {
      return new CreditBindingItem {
        CreditNo = credit.CreditNo.ToString(),
        Artist = credit.Artist.Name,
        Role = credit.Role.Name
      };
    }

    protected override TypedBindingList<Credit, CreditBindingItem> CreateBindingList(
      IList<CreditBindingItem> list) {
      return new CreditBindingList(list);
    }

    protected override BindingColumnList CreateColumns() {
      var result = new BindingColumnList {
        new BindingColumn(nameof(Credit.CreditNo), typeof(int)) {IsInKey = true},
        new BindingColumn(nameof(Credit.Artist), typeof(string),
          new ReferenceType(typeof(ArtistList), nameof(Artist.Name))),
        new BindingColumn(nameof(Credit.Role), typeof(string),
          new ReferenceType(typeof(RoleList), nameof(Role.Name)))
      };
      return result;
    }

    public override IEntityList CreateParentList() {
      return new PieceList(false) {Session = Session};
    }
  }
}