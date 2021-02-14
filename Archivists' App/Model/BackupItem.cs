using System.Collections.Generic;
using System.Linq;

namespace SoundExplorers.Model {
  public class BackupItem<TBindingItem> where TBindingItem : IBindingItem, new() {
    public BackupItem(TBindingItem bindingItem) {
      Dictionary = new Dictionary<string, object?>();
      foreach (var propertyName in bindingItem.Properties.Keys) {
        Dictionary.Add(propertyName, bindingItem.GetPropertyValue(propertyName));
      }
    }

    private IDictionary<string, object?> Dictionary { get; }

    public TBindingItem CreateBindingItem() {
      var result = new TBindingItem();
      RestoreTo(result);
      return result;
    }

    public IList<object?> GetValues() {
      return Dictionary.Values.ToList();
    }

    public object? GetPropertyValue(string propertyName) {
      return Dictionary[propertyName];
    }

    private void RestoreTo(TBindingItem bindingItem) {
      foreach (var (key, value) in Dictionary) {
        bindingItem.SetPropertyValue(key, value);
      }
    }
  }
}