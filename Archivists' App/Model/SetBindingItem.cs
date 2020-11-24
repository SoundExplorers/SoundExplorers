using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class SetBindingItem : BindingItemBase<Set, SetBindingItem> {
    private DateTime _date;
    private string _location;
    private int _setNo;
    private string _act;
    private string _genre;
    private string _notes;

    public DateTime Date {
      get => _date;
      set {
        _date = value;
        OnPropertyChanged(nameof(Date));
      }
    }

    public string Location {
      get => _location;
      set {
        _location = value;
        OnPropertyChanged(nameof(Location));
      }
    }

    public int SetNo {
      get => _setNo;
      set {
        _setNo = value;
        OnPropertyChanged(nameof(SetNo));
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

    internal override Key GetKey() {
      // This won't work.  We are not (yet) providing access to event parents.
      // They would be the list.
      return new Key(Set.SetNoToSimpleKey(SetNo), FindParent(Properties[nameof(Event)]));
    }
  }
}