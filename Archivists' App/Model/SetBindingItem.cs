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

    protected override IDictionary<string, object?>
      CreateEntityPropertyValueDictionary() {
      if (!EntityList.IsChildList && !EntityList.IsInsertionRowCurrent) {
        return base.CreateEntityPropertyValueDictionary();
      }
      var @event = EntityList.IdentifyingParent!;
      return new Dictionary<string, object?> {
        [nameof(Event)] = QueryHelper.Read<Event>(@event.SimpleKey,
          @event.IdentifyingParent, EntityList.Session),
        [nameof(SetNo)] = SetNo,
        [nameof(Act)] =
          Act != null ? QueryHelper.Read<Act>(Act, EntityList.Session) : null,
        [nameof(Genre)] = Genre != null
          ? QueryHelper.Read<Genre>(Genre, EntityList.Session)
          : null,
        [nameof(IsPublic)] = IsPublic,
        [nameof(Notes)] = Notes
      };
    }

    // protected override Set? CreateEntityFromProperties() {
    //   if (!EntityList.IsChildList && !EntityList.IsInsertionRowCurrent) {
    //     return null;
    //   }
    //   var actParent = 
    //     Act != null ? QueryHelper.Read<Act>(Act, EntityList.Session) : null;
    //   var genreParent =
    //     Genre != null ? QueryHelper.Read<Genre>(Act, EntityList.Session) : null;
    //   var result = new Set {
    //     Event = (Event)EntityList.IdentifyingParent!, SetNo = SetNo, Act = actParent,
    //     IsPublic = IsPublic, Notes = Notes
    //   };
    //   if (genreParent != null) {
    //     result.Genre = genreParent;
    //   }
    //   return result;
    // }

    protected override void CopyValuesToEntityProperties(Set set) {
      base.CopyValuesToEntityProperties(set);
      set.Event = (Event)EntityList.IdentifyingParent!;
    }

    protected override string GetSimpleKey() {
      return EntityBase.IntegerToSimpleKey(SetNo, nameof(SetNo));
    }
  }
}