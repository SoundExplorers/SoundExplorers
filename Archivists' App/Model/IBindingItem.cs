using JetBrains.Annotations;

namespace SoundExplorers.Model {
  internal interface IBindingItem {
    void SetPropertyValue([NotNull] string propertyName, [CanBeNull] object value);
  }
}