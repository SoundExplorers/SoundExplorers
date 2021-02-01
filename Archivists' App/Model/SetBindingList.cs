using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetBindingList : TypedBindingList<Set, SetBindingItem> {
    public SetBindingList(IList<SetBindingItem> bindingItems) :
      base(bindingItems) { }

    protected override void OnAddingNew(AddingNewEventArgs e) {
      base.OnAddingNew(e);
      e.NewObject ??= new SetBindingItem {SetNo = GetDefaultSetNo()};
    }

    private int GetDefaultSetNo() {
      return Count > 0
        ? (from setBindingItem in Items select setBindingItem.SetNo).Max() + 1
        : 1;
    }
  }
}