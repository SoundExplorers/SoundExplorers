using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class SetBindingItem : BindingItemBase<Set, SetBindingItem> {
    private DateTime _date;
    private string _location = null!;
    private int _setNo;
    private string? _act;
    private string? _genre;
    private bool _isPublic;
    private string _notes = null!;

    public SetBindingItem() {
      Act = Set.DefaultActName;
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

    public int SetNo {
      get => _setNo;
      set {
        _setNo = value;
        OnPropertyChanged(nameof(SetNo));
      }
    }

    public string? Act {
      get => _act;
      set {
        _act = value;
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
      if (!EntityList.IsChildList && !EntityList.IsInsertionRowCurrent) {
        return base.CreateEntityPropertyValueDictionary();
      }
      Event = ReadEvent();
      return new Dictionary<string, object?> {
        [nameof(Date)] = Event.Date,
        [nameof(Location)] = ReadLocation(),
        [nameof(SetNo)] = SetNo,
        [nameof(Act)] = ReadAct(),
        [nameof(Genre)] = ReadGenre(),
        [nameof(IsPublic)] = IsPublic,
        [nameof(Notes)] = Notes
      };
    }

    protected override void CopyValuesToEntityProperties(Set set) {
      // The order is crucial here. SetNo must be set before Genre and Event so that
      // Genre.Sets and Event.Sets will be in the right sort order. And, to avoid
      // referential integrity exceptions, Genre must be set before Event,
      // which must be set before Act.
      set.SetNo = SetNo; 
      set.Genre = (Genre)FindParent(Properties[nameof(Genre)])!;
      set.Event = Event;
      set.Act = (Act)FindParent(Properties[nameof(Act)])!;
      set.IsPublic = IsPublic;
      set.Notes = Notes;
    }

    // internal override void RestorePropertyValuesFrom(SetBindingItem backup) {
    //   // base.RestorePropertyValuesFrom(backup);
    //   // Event = (Event)EntityList.IdentifyingParent!;
    //   // if (!EntityList.IsChildList && !EntityList.IsInsertionRowCurrent) {
    //   //   base.RestorePropertyValuesFrom(backup);
    //   // }
    //   Event = (Event)EntityList.IdentifyingParent!;
    //   Date = Event.Date;
    //   Location = Event.Location.Name;
    //   SetNo = backup.SetNo;
    //   Act = backup.Act;
    //   Genre = backup.Genre;
    //   IsPublic = backup.IsPublic;
    //   Notes = backup.Notes;
    // }

    protected override string GetSimpleKey() {
      return EntityBase.IntegerToSimpleKey(SetNo, nameof(SetNo));
    }

    private Act? ReadAct() {
      return Act != null
        ? QueryHelper.Read<Act>(Act, EntityList.Session)
        : null;
    }

    private Event ReadEvent() {
      var listEvent = EntityList.IdentifyingParent!;
      return QueryHelper.Read<Event>(
        listEvent.SimpleKey, listEvent.IdentifyingParent, EntityList.Session);
    }

    private Genre? ReadGenre() {
      return Genre != null ? QueryHelper.Read<Genre>(Genre, EntityList.Session) : null;
    }

    private Location ReadLocation() {
      return QueryHelper.Read<Location>(Event.Location.Name, EntityList.Session);
    }
  }
}