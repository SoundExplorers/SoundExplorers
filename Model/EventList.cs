using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event, EventBindingItem> {
    private bool HasDefaultEventTypeBeenFound { get; set; }
    private bool HasDefaultNewsletterBeenFound { get; set; }
    private bool HasDefaultSeriesBeenFound { get; set; }

    private void AddDefaultEventTypeIfItDoesNotExist() {
      Session.BeginUpdate();
      var defaultEventType = QueryHelper.Find<EventType>(
        EventType.DefaultName, Session);
      if (defaultEventType == null) {
        defaultEventType = EventType.CreateDefault();
        Session.Persist(defaultEventType);
      }
      Session.Commit();
      HasDefaultEventTypeBeenFound = true;
    }

    private void AddDefaultNewsletterIfItDoesNotExist() {
      Session.BeginUpdate();
      var defaultNewsletter = QueryHelper.Find<Newsletter>(
        EntityBase.DateToSimpleKey(EntityBase.DefaultDate), Session);
      if (defaultNewsletter == null) {
        defaultNewsletter = Newsletter.CreateDefault();
        Session.Persist(defaultNewsletter);
      }
      Session.Commit();
      HasDefaultNewsletterBeenFound = true;
    }

    private void AddDefaultSeriesIfItDoesNotExist() {
      Session.BeginUpdate();
      var defaultSeries = QueryHelper.Find<Series>(Series.DefaultName, Session);
      if (defaultSeries == null) {
        defaultSeries = Series.CreateDefault();
        Session.Persist(defaultSeries);
      }
      Session.Commit();
      HasDefaultSeriesBeenFound = true;
    }

    protected override IComparer<Event> CreateEntityComparer() {
      return new EventComparer();
    }

    protected override EventBindingItem CreateBindingItem(Event @event) {
      return new EventBindingItem {
        Date = @event.Date, Location = @event.Location.Name!,
        Series = @event.Series.Name,
        Newsletter = @event.Newsletter.Date,
        EventType = @event.EventType.Name,
        Notes = @event.Notes
      };
    }

    protected override BindingColumnList CreateColumns() {
      return new BindingColumnList {
        new BindingColumn(nameof(Event.Date), typeof(DateTime)) {IsInKey = true},
        new BindingColumn(nameof(Event.Location), typeof(string),
          new ReferenceType(typeof(LocationList), nameof(Location.Name))) {
          IsInKey = true
        },
        new BindingColumn(nameof(Event.Newsletter), typeof(DateTime),
          new ReferenceType(typeof(NewsletterList), nameof(Newsletter.Date))),
        new BindingColumn(nameof(Event.EventType), typeof(string),
          new ReferenceType(typeof(EventTypeList), nameof(EventType.Name))),
        new BindingColumn(nameof(Event.Series), typeof(string),
          new ReferenceType(typeof(SeriesList), nameof(Series.Name))),
        new BindingColumn(nameof(Event.Notes), typeof(string))
      };
    }

    protected override IList GetChildList(int rowIndex) {
      return this[rowIndex].Sets.Values.ToList();
    }

    public override void Populate(
      IdentifyingParentAndChildren? identifyingParentChildren = null,
      bool createBindingList = true) {
      if (!HasDefaultEventTypeBeenFound) {
        AddDefaultEventTypeIfItDoesNotExist();
      }
      if (!HasDefaultNewsletterBeenFound) {
        AddDefaultNewsletterIfItDoesNotExist();
      }
      if (!HasDefaultSeriesBeenFound) {
        AddDefaultSeriesIfItDoesNotExist();
      }
      base.Populate(identifyingParentChildren, createBindingList);
    }
  }
}