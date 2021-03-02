using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class CreditBindingList : TypedBindingList<Credit, CreditBindingItem> {
    public CreditBindingList(IList<CreditBindingItem> bindingItems) :
      base(bindingItems) { }

    private string GetDefaultCreditNo() {
      return CreditBindingItem.GetDefaultIntegerSimpleKey(
        (from item in Items select item.CreditNo).ToList());
    }

    protected override void OnAddingNew(AddingNewEventArgs e) {
      base.OnAddingNew(e);
      e.NewObject ??= new CreditBindingItem {CreditNo = GetDefaultCreditNo()};
    }
  }
}