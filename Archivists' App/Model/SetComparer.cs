using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetComparer : Comparer<Set> {
    public override int Compare(Set? set1, Set? set2) {
      var event1 = (set1!.IdentifyingParent as Event)!;
      var event2 = (set2!.IdentifyingParent as Event)!;
      // Compare Dates first.
      if (event1.Date < event2.Date) {
        return -1;
      }
      if (event1.Date > event2.Date) {
        return 1;
      }
      // Same Date. Compare Locations. 
      if (event1.Location.Key < event2.Location.Key) {
        return -1;
      }
      return event1.Location.Key > event2.Location.Key
        ? 1
        // Same Event. Compare SetNos. 
        : Key.CompareSimpleKeys(set1!.Key, set2!.Key);
    }
  }
}