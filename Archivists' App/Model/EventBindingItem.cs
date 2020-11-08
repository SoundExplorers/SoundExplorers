using System;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  [NoReorder]
  public class EventBindingItem : BindingItemBase<Event, EventBindingItem> {
    private const string DefaultEventTypeName = "Performance";
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
      //get => _eventType ?? (_eventType = GetDefaultEventTypeName());
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

    // internal override void CopyPropertyValuesToEntity(Event @event) {
    //   @event.Date = Date;
    //   @event.Location = QueryHelper.Find<Location>(Location, Session);
    //   @event.EventType = QueryHelper.Find<EventType>(EventType, Session);
    //   @event.Newsletter = Newsletter > EntityBase.InitialDate
    //     ? QueryHelper.Find<Newsletter>(EntityBase.DateToSimpleKey(Newsletter), Session)
    //     : null;
    //   @event.Series = QueryHelper.Find<Series>(Series, Session);
    //   @event.Notes = Notes;
    // }

    // protected override void CopyPropertyValueToEntity(PropertyInfo property, Event @event,
    //   IEnumerable<PropertyInfo> entityProperties) {
    //   if (property.Name == nameof(Date)) {
    //     @event.Date = Date;
    //   } else {
    //     base.CopyPropertyValueToEntity(property, @event, entityProperties);
    //   }
    // }

    // protected override void SetEntityProperty(Event @event, PropertyInfo entityProperty,
    //   object newEntityPropertyValue) {
    //   if (entityProperty.Name == nameof(Date)) {
    //     @event.Date = (DateTime?)newEntityPropertyValue ??
    //                   throw new ArgumentNullException(nameof(Date));
    //   } else {
    //     base.SetEntityProperty(@event, entityProperty, newEntityPropertyValue);
    //   }
    // }
  }
}