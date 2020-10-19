﻿using System;
using System.Collections;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event, EventBindingItem> {
    public override IList GetChildren(int rowIndex) {
      return (IList)this[rowIndex].Sets.Values;
    }

    protected override Event CreateBackupEntity(Event @event) {
      return new Event {
        Date = @event.Date, Location = @event.Location, Series = @event.Series,
        Newsletter = @event.Newsletter, EventType = @event.EventType,
        Notes = @event.Notes
      };
    }

    protected override EventBindingItem CreateBindingItem(Event @event) {
      return new EventBindingItem {
        Date = @event.Date, Location = @event.Location, Series = @event.Series,
        Newsletter = @event.Newsletter, EventType = @event.EventType, Notes = @event.Notes
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Event.Date)),
        new EntityColumn(nameof(Event.Location),
          typeof(Location), nameof(Location.Name)),
        new EntityColumn(nameof(Event.Series),
          typeof(Series), nameof(Series.Name)),
        new EntityColumn(nameof(Event.Newsletter),
          typeof(Newsletter), nameof(Newsletter.Date)),
        new EntityColumn(nameof(Event.EventType),
          typeof(EventType), nameof(EventType.Name)),
        new EntityColumn(nameof(Event.Notes))
      };
    }

    protected override Event CreateEntity(EventBindingItem bindingItem) {
      return new Event {
        Date = bindingItem.Date, Location = bindingItem.Location,
        Series = bindingItem.Series,
        Newsletter = bindingItem.Newsletter, EventType = bindingItem.EventType,
        Notes = bindingItem.Notes
      };
    }

    protected override void RestoreEntityPropertiesFromBackup(Event backupEvent,
      Event eventToRestore) {
      eventToRestore.Date = backupEvent.Date;
      eventToRestore.Location = backupEvent.Location;
      eventToRestore.Series = backupEvent.Series;
      eventToRestore.Newsletter = backupEvent.Newsletter;
      eventToRestore.EventType = backupEvent.EventType;
      eventToRestore.Notes = backupEvent.Notes;
    }

    protected override void UpdateEntityProperty(string propertyName, object newValue,
      Event @event) {
      switch (propertyName) {
        case nameof(@event.Date):
          @event.Date = (DateTime?)newValue ??
                        throw new NullReferenceException(nameof(@event.Date));
          break;
        case nameof(@event.Location):
          @event.Location = newValue as Location;
          break;
        case nameof(@event.Series):
          @event.Series = newValue as Series;
          break;
        case nameof(@event.Newsletter):
          @event.Newsletter = newValue as Newsletter;
          break;
        case nameof(@event.EventType):
          @event.EventType = newValue as EventType;
          break;
        case nameof(@event.Notes):
          @event.Notes = newValue?.ToString();
          break;
        default:
          throw new ArgumentException(
            $"{nameof(propertyName)} '{propertyName}' is not supported.",
            nameof(propertyName));
      }
    }
  }
}