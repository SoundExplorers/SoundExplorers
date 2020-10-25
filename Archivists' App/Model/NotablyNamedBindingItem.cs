using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NotablyNamedBindingItem<TEntity> 
    : BindingItemBase<TEntity, NotablyNamedBindingItem<TEntity>>
    where TEntity : EntityBase, INotablyNamedEntity, new() {
    private string _name;
    private string _notes;

    public string Name {
      get => _name;
      set {
        _name = value;
        OnPropertyChanged(nameof(Name));
      }
    }

    public string Notes {
      get => _notes;
      set {
        _notes = value;
        OnPropertyChanged(nameof(Notes));
      }
    }
  }
}