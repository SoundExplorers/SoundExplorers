using JetBrains.Annotations;

namespace SoundExplorers.Model {
  [NoReorder]
  public class NamedBindingItem : BindingItemBase<NamedBindingItem> {
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