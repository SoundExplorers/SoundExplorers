using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class EventBindingItem : BindingItemBase<Event, EventBindingItem> {
    internal const string DefaultEventTypeName = "Performance";
    private DateTime _date;
    private string _location;
    private string _series;
    private DateTime _newsletter;
    private string _eventType;
    private string _notes;

    public EventBindingItem() {
      Date = DateTime.Today;
      Newsletter = EntityBase.InitialDate;
      EventType = DefaultEventTypeName;
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

    public DateTime Newsletter {
      get => _newsletter;
      set {
        _newsletter = value;
        OnPropertyChanged(nameof(Newsletter));
      }
    }

    public string EventType {
      get => _eventType;
      set {
        _eventType = value;
        OnPropertyChanged(nameof(EventType));
      }
    }

    public string Series {
      get => _series;
      set {
        _series = value;
        OnPropertyChanged(nameof(Series));
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
      return new Key(EntityBase.DateToSimpleKey(Date),
        FindParent(Properties[nameof(Location)]));
    }
  }
}