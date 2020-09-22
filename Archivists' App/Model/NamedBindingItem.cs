namespace SoundExplorers.Model {
  public class NamedBindingItem : BindingItemBase {
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