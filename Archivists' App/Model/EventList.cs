using System.Collections;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventList : EntityListBase<Event, EventBindingItem> {
    public override IList GetChildrenForMainList(int rowIndex) {
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

    protected override BindingColumnList CreateColumns() {
      return new BindingColumnList {
        new BindingColumn(nameof(Event.Date)),
        new BindingColumn(nameof(Event.Location),
          typeof(LocationList), nameof(Location.Name)),
        new BindingColumn(nameof(Event.Newsletter),
          typeof(NewsletterList), nameof(Newsletter.Date)),
        new BindingColumn(nameof(Event.EventType),
          typeof(EventTypeList), nameof(EventType.Name)),
        new BindingColumn(nameof(Event.Series),
          typeof(SeriesList), nameof(Series.Name)),
        new BindingColumn(nameof(Event.Notes))
      };
    }
  }
}