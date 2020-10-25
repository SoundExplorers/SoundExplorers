using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NamedBindingItem<TEntity>
    : BindingItemBase<TEntity, NamedBindingItem<TEntity>>
    where TEntity : EntityBase, INamedEntity, new() {
    private string _name;

    public string Name {
      get => _name;
      set {
        _name = value;
        OnPropertyChanged(nameof(Name));
      }
    }
  }
}