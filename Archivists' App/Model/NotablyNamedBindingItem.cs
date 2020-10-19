using JetBrains.Annotations;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NotablyNamedBindingItem : BindingItemBase<NotablyNamedBindingItem> {
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