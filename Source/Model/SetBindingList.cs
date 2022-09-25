using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model; 

public class SetBindingList : TypedBindingList<Set, SetBindingItem> {
  public SetBindingList(IList<SetBindingItem> bindingItems) :
    base(bindingItems) { }

  private string GetDefaultSetNo() {
    return SetBindingItem.GetDefaultIntegerSimpleKey(
      (from item in Items select item.SetNo).ToList());
  }

  protected override void OnAddingNew(AddingNewEventArgs e) {
    base.OnAddingNew(e);
    e.NewObject ??= new SetBindingItem {SetNo = GetDefaultSetNo()};
  }
}