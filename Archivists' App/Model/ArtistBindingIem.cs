using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class ArtistBindingItem
    : BindingItemBase<Artist, ArtistBindingItem> {
    private string _surname = null!;
    private string _forename = null!;
    private string _notes = null!;

    public string Surname {
      get => _surname;
      set {
        _surname = value;
        OnPropertyChanged(nameof(Surname));
      }
    }

    public string Forename {
      get => _forename;
      set {
        _forename = value;
        OnPropertyChanged(nameof(Forename));
      }
    }

    public string Notes {
      get => _notes;
      set {
        _notes = value;
        OnPropertyChanged(nameof(Notes));
      }
    }

    protected override string GetSimpleKey() {
      return Artist.MakeName(Forename, Surname);
    }
  }
}