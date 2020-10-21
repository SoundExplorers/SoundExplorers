using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public interface IBindingItem {
    void SetParent([NotNull] string propertyName, [CanBeNull] IEntity parent);
  }
}