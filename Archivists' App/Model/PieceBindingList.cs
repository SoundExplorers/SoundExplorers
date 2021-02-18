using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class PieceBindingList : TypedBindingList<Piece, PieceBindingItem> {
    public PieceBindingList(IList<PieceBindingItem> bindingItems) :
      base(bindingItems) { }

    protected override void OnAddingNew(AddingNewEventArgs e) {
      base.OnAddingNew(e);
      e.NewObject ??= new PieceBindingItem {PieceNo = GetDefaultPieceNo()};
    }

    private int GetDefaultPieceNo() {
      return Count > 0
        ? (from pieceBindingItem in Items select pieceBindingItem.PieceNo).Max() + 1
        : 1;
    }
  }
}