using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class SetBindingItem : BindingItemBase<Set, SetBindingItem> {
    private int _setNo;
    private string _act;
    private string _genre;
    private string _notes;

    public int SetNo {
      get => _setNo;
      set {
        _setNo = value;
        OnPropertyChanged(nameof(Genre));
      }
    }
    
    public string Act {
      get => _act;
      set {
        _act = value;
        OnPropertyChanged(nameof(Act));
      }
    }
    
    public string Genre {
      get => _genre;
      set {
        _genre = value;
        OnPropertyChanged(nameof(Genre));
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