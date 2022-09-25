using System.Collections.Generic;
using System.Reflection;

namespace SoundExplorers.Model; 

public interface IBindingItem {
  IDictionary<string, PropertyInfo> Properties { get; }
  object? GetPropertyValue(string propertyName);
  void SetPropertyValue(string propertyName, object? value);
}