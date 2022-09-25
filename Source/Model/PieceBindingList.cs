using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model; 

public class PieceBindingList : TypedBindingList<Piece, PieceBindingItem> {
  public PieceBindingList(IList<PieceBindingItem> bindingItems) :
    base(bindingItems) { }

  private string GetDefaultPieceNo() {
    return PieceBindingItem.GetDefaultIntegerSimpleKey(
      (from item in Items select item.PieceNo).ToList());
  }

  protected override void OnAddingNew(AddingNewEventArgs e) {
    base.OnAddingNew(e);
    e.NewObject ??= new PieceBindingItem {PieceNo = GetDefaultPieceNo()};
  }
}