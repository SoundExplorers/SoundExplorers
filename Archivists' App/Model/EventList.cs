using System;
using System.Collections;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event, EventBindingItem> {
    private bool HasDefaultEventTypeBeenFound { get; set; }

    private void AddDefaultEventTypeIfItDoesNotExist() {
      Session.BeginUpdate();
      var defaultEventType = QueryHelper.Find<EventType>(
        EventBindingItem.DefaultEventTypeName, Session);
      if (defaultEventType == null) {
        defaultEventType = new EventType {
          Name = EventBindingItem.DefaultEventTypeName
        };
        Session.Persist(defaultEventType);
      }
      Session.Commit();
      HasDefaultEventTypeBeenFound = true;
    }

    protected override EventBindingItem CreateBindingItem(Event @event) {
      return new() {
        Date = @event.Date, Location = @event.Location.Name!,
        Series = @event.Series?.Name,
        Newsletter = @event.Newsletter?.Date ?? EntityBase.InitialDate,
        EventType = @event.EventType.Name,
        Notes = @event.Notes
      };
    }

    protected override BindingColumnList CreateColumns() {
      return new() {
        new BindingColumn(nameof(Event.Date), typeof(DateTime)) {IsInKey = true},
        new BindingColumn(nameof(Event.Location), typeof(string),
          typeof(LocationList), nameof(Location.Name)) {IsInKey = true},
        new BindingColumn(nameof(Event.Newsletter), typeof(DateTime),
          typeof(NewsletterList), nameof(Newsletter.Date)),
        new BindingColumn(nameof(Event.EventType),typeof(string),
          typeof(EventTypeList), nameof(EventType.Name)),
        new BindingColumn(nameof(Event.Series), typeof(string),
          typeof(SeriesList), nameof(Series.Name)),
        new BindingColumn(nameof(Event.Notes), typeof(string))
      };
    }

    public override IList GetChildrenForMainList(int rowIndex) {
      return this[rowIndex].Sets.Values.ToList();
    }

    public override void Populate(IList? list = null, bool createBindingList = true) {
      if (!HasDefaultEventTypeBeenFound) {
        AddDefaultEventTypeIfItDoesNotExist();
      }
      base.Populate(list, createBindingList);
    }
  }
}