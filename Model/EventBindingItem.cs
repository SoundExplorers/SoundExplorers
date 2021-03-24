using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class EventBindingItem : BindingItemBase<Event, EventBindingItem> {
    private DateTime _date;
    private string _location = null!;
    private string? _series;
    private DateTime _newsletter;
    private string _eventType = null!;
    private string _notes = null!;

    public EventBindingItem() {
      Date = DateTime.Today;
      Newsletter = EntityBase.DefaultDate;
      EventType = Data.EventType.DefaultName;
      Series = Data.Series.DefaultName;
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

    public string? Series {
      get => _series;
      set {
        _series = !string.IsNullOrWhiteSpace(value)
          ? value
          : Data.Series.DefaultName;
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

    internal override void ValidateInsertion() {
      base.ValidateInsertion();
      ValidateLocationOnInsertion();
    }

    protected override Key CreateKey() {
      return new Key(GetSimpleKey(), FindParent(Properties[nameof(Location)]));
    }

    protected override string GetSimpleKey() {
      return EntityBase.DateToSimpleKey(Date);
    }

    private void ValidateLocationOnInsertion() {
      if (string.IsNullOrWhiteSpace(Location)) {
        throw EntityBase.CreateParentNotSpecifiedException(
          nameof(Event), Key, nameof(Location));
      }
    }
  }
}