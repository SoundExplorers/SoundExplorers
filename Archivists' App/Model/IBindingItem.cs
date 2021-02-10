using System.Collections.Generic;
using System.Reflection;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public interface IBindingItem {
    IDictionary<string, PropertyInfo> Properties { get; }
    Key CreateKey();
    object? GetPropertyValue(string propertyName);
    void SetPropertyValue(string propertyName, object? value);
  }
}