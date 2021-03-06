using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventComparer : Comparer<Event> {
    public override int Compare(Event? event1, Event? event2) {
      // Compare Dates first.
      if (event1!.Date < event2!.Date) {
        return -1;
      }
      return event1.Date > event2.Date
        ? 1
        // Same Date. Compare Locations.
        : Key.CompareSimpleKeys(event1.Location.SimpleKey, event2.Location.SimpleKey);
    }
  }
}