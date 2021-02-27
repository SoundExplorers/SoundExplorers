using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class SetBindingItem : BindingItemBase<Set, SetBindingItem> {
    private DateTime _date;
    private string _location = null!;
    private string _setNo = null!;
    private string? _act;
    private string? _genre;
    private bool _isPublic;
    private string _notes = null!;

    public SetBindingItem() {
      Act = Data.Act.DefaultName;
      IsPublic = true;
    }

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

    public string SetNo {
      get => _setNo;
      set {
        _setNo = value;
        OnPropertyChanged(nameof(SetNo));
      }
    }

    public string? Act {
      get => _act;
      set {
        _act = !string.IsNullOrWhiteSpace(value)
          ? value
          : Data.Act.DefaultName;
        OnPropertyChanged(nameof(Act));
      }
    }

    public string? Genre {
      get => _genre;
      set {
        _genre = value;
        OnPropertyChanged(nameof(Genre));
      }
    }

    public bool IsPublic {
      get => _isPublic;
      set {
        _isPublic = value;
        OnPropertyChanged(nameof(IsPublic));
      }
    }

    public string Notes {
      get => _notes;
      set {
        _notes = value;
        OnPropertyChanged(nameof(Notes));
      }
    }

    private Event Event { get; set; } = null!;

    protected override IDictionary<string, object?>
      CreateEntityPropertyValueDictionary() {
      Event = (EntityList.IdentifyingParent as Event)!;
      return new Dictionary<string, object?> {
        [nameof(Date)] = Event.Date,
        [nameof(Location)] = Event.Location,
        [nameof(SetNo)] = SetNo,
        [nameof(Act)] = Act,
        [nameof(Genre)] = Genre,
        [nameof(IsPublic)] = IsPublic,
        [nameof(Notes)] = Notes
      };
    }

    protected override void CopyValuesToEntityProperties(Set set) {
      // SetNo must be set before Genre and Event so that Genre.Sets and Event.Sets will
      // have the correct key for the Set and therefore be in the right sort order. And,
      // to avoid VelocityDB exceptions (I am now not sure what), Genre must be set
      // before Event, which must be set before Act.
      set.SetNo = SimpleKeyToInteger(SetNo, nameof(SetNo));
      set.Genre = (Genre)FindParent(Properties[nameof(Genre)])!;
      set.Event = Event;
      set.Act = (Act)FindParent(Properties[nameof(Act)])!;
      set.IsPublic = IsPublic;
      set.Notes = Notes;
    }

    protected override object? GetEntityPropertyValue(PropertyInfo property,
      PropertyInfo entityProperty) {
      switch (property.Name) {
        case nameof(SetNo):
          string setNoString = GetPropertyValue(nameof(SetNo))!.ToString()!;
          return SimpleKeyToInteger(setNoString, nameof(SetNo));
        default:
          return base.GetEntityPropertyValue(property, entityProperty);
      }
    }

    protected override string GetSimpleKey() {
      // Validate and format the SetNo, which may have been entered by the user.
      return EntityBase.IntegerToSimpleKey(SimpleKeyToInteger(
          SetNo, nameof(SetNo)),
        nameof(SetNo));
    }

    private void ValidateGenreOnInsertion() {
      if (string.IsNullOrWhiteSpace(Genre)) {
        throw EntityBase.CreateParentNotSpecifiedException(
          nameof(Set), Key, nameof(Genre));
      }
    }

    internal override void ValidateInsertion() {
      base.ValidateInsertion();
      ValidateGenreOnInsertion();
    }
  }
}