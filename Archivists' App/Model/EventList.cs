using System;
using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event> {
    protected override Event CreateBackupEntity(Event @event) {
      return new Event {
        Date = @event.Date, Location = @event.Location, Series = @event.Series,
        Newsletter = @event.Newsletter, EventType = @event.EventType, Notes = @event.Notes
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Event.Date), typeof(DateTime)),
        new EntityColumn(nameof(Event.Location), typeof(string),
          nameof(Location), nameof(Location.Name)),
        new EntityColumn(nameof(Event.Series), typeof(string),
          nameof(Series), nameof(Series.Name)),
        new EntityColumn(nameof(Event.Newsletter), typeof(DateTime),
          nameof(Newsletter), nameof(Newsletter.Date)),
        new EntityColumn(nameof(Event.EventType), typeof(string),
          nameof(EventType), nameof(EventType.Name)),
        new EntityColumn(nameof(Event.Notes), typeof(string))
      };
    }

    protected override IList<object> GetRowItemValuesFromEntity(Event @event) {
      return new List<object> {
        @event.Date, @event.Location.Name, @event.Series?.Name,
        @event.Newsletter?.Date ?? DateTime.Parse("1 Jan 1900"), @event.EventType.Name,
        @event.Notes
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

    protected override void UpdateEntityAtRow(int rowIndex) {
      var row = Table.Rows[rowIndex];
      var newDate = (DateTime)row[nameof(Event.Date)];
      var newLocationName = row[nameof(Event.Location)].ToString();
      var newSeriesName = row[nameof(Event.Series)].ToString();
      var newNewsletterDate = (DateTime)row[nameof(Event.Newsletter)];
      var newEventTypeName = row[nameof(Event.EventType)].ToString();
      var newNotes = row[nameof(Event.Notes)].ToString();
      var @event = this[rowIndex];
      if (newDate != @event.Date) {
        @event.Date = newDate;
      }
      if (newLocationName != @event.Location.Name) {
        @event.Location = QueryHelper.Read<Location>(newLocationName, Session);
      }
      if (newSeriesName != @event.Series?.Name) {
        @event.Series = !string.IsNullOrEmpty(newSeriesName)
          ? QueryHelper.Read<Series>(newSeriesName, Session)
          : null;
      }
      if ((newNewsletterDate == DateTime.MinValue ||
           newNewsletterDate == DateTime.Parse("1 Jan 1900")) &&
          @event.Newsletter != null) {
        @event.Newsletter = null;
      } else if (newNewsletterDate != @event.Newsletter?.Date) {
        @event.Newsletter =
          QueryHelper.Read<Newsletter>($"{newNewsletterDate:yyyy/MM/dd}", Session);
      }
      if (newEventTypeName != @event.EventType.Name) {
        @event.EventType = QueryHelper.Read<EventType>(newEventTypeName, Session);
      }
      if (newNotes != @event.Notes) {
        @event.Notes = newNotes;
      }
    }
  }
}