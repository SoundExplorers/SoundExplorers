using System;

namespace SoundExplorers.Model {
  public class SetBackupItem : BackupItem<SetBindingItem> {
    public SetBackupItem(SetBindingItem bindingItem) : base(bindingItem) { }

    public override void RestoreTo(SetBindingItem bindingItem) {
      bindingItem.Date = (DateTime)GetPropertyValue(nameof(SetBindingItem.Date))!;
      bindingItem.Location =
        GetPropertyValue(nameof(SetBindingItem.Location))!.ToString()!;
      bindingItem.SetNo = (int)GetPropertyValue(nameof(SetBindingItem.SetNo))!;
      bindingItem.Act = GetPropertyValue(nameof(SetBindingItem.Act))!.ToString();
      bindingItem.Genre = GetPropertyValue(nameof(SetBindingItem.Genre))!.ToString()!;
      bindingItem.IsPublic = (bool)GetPropertyValue(nameof(SetBindingItem.IsPublic))!;
      bindingItem.Notes = GetPropertyValue(nameof(SetBindingItem.Notes))!.ToString()!;
    }
  }
}