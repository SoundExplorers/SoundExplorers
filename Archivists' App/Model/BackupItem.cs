using System.Collections.Generic;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class BackupItem<TBindingItem> where TBindingItem : IBindingItem, new() {
    public BackupItem(TBindingItem bindingItem) {
      Dictionary = new Dictionary<string, object?>();
      foreach (var propertyName in bindingItem.Properties.Keys) {
        Dictionary.Add(propertyName, bindingItem.GetPropertyValue(propertyName));
      }
      try {
        Key = bindingItem.CreateKey();
      } catch {
        // Null if invalid
      }
    }
    
    private IDictionary<string, object?> Dictionary { get; }

    public object? this[string propertyName] => Dictionary[propertyName];
    
    /// <summary>
    ///   Null if a valid key cannot be constructed.
    /// </summary>
    public Key? Key { get; }

    public TBindingItem CreateBindingItem() {
      var result = new TBindingItem();
      RestoreTo(result);
      return result;
    }

    public IList<object?> GetValues() {
      return Dictionary.Values.ToList();
    }

    public virtual void RestoreTo(TBindingItem bindingItem) {
      foreach (var (key, value) in Dictionary) {
        bindingItem.SetPropertyValue(key, value);
      }
    }
  }
}