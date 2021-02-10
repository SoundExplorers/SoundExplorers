using System;

namespace SoundExplorers.Model {
  public class SetBackupItem : BackupItem<SetBindingItem> {
    public SetBackupItem(SetBindingItem bindingItem) : base(bindingItem) { }
    public override void RestoreTo(SetBindingItem bindingItem) {
      bindingItem.Date = (DateTime)this[nameof(SetBindingItem.Date)]!;
      bindingItem.Location = this[nameof(SetBindingItem.Location)]!.ToString()!;
      bindingItem.SetNo = (int)this[nameof(SetBindingItem.SetNo)]!;
      bindingItem.Act = this[nameof(SetBindingItem.Act)]!.ToString();
      bindingItem.Genre = this[nameof(SetBindingItem.Genre)]!.ToString()!;
      bindingItem.IsPublic = (bool)this[nameof(SetBindingItem.IsPublic)]!;
      bindingItem.Notes = this[nameof(SetBindingItem.Notes)]!.ToString()!;
    }
  }
}