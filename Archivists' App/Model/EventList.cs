using System.Collections;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event, EventBindingItem> {
    public EventList() : base(typeof(LocationList)) { }

    public override IList GetChildren(int rowIndex) {
      return this[rowIndex].Sets.Values.ToList();
    }

    protected override EventBindingItem CreateBindingItem(Event @event) {
      return new EventBindingItem {
        Date = @event.Date, Location = @event.Location.Name,
        Series = @event.Series?.Name,
        Newsletter = @event.Newsletter?.Date ?? EntityBase.InitialDate,
        EventType = @event.EventType.Name,
        Notes = @event.Notes
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Event.Date)),
        new EntityColumn(nameof(Event.Location),
          typeof(LocationList), nameof(Location.Name)),
        new EntityColumn(nameof(Event.Newsletter),
          typeof(NewsletterList), nameof(Newsletter.Date)),
        new EntityColumn(nameof(Event.EventType),
          typeof(EventTypeList), nameof(EventType.Name)),
        new EntityColumn(nameof(Event.Series),
          typeof(SeriesList), nameof(Series.Name)),
        new EntityColumn(nameof(Event.Notes))
      };
    }
  }
}