using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetComparer : Comparer<Set> {
    public override int Compare(Set? set1, Set? set2) {
      var event1 = (set1!.IdentifyingParent as Event)!;
      var event2 = (set2!.IdentifyingParent as Event)!;
      // Compare Events first.
      var eventComparer = new EventComparer();
      int eventComparison = eventComparer.Compare(event1, event2);
      return eventComparison != 0
        ? eventComparison
        // Same Event. Compare SetNos. 
        : Key.CompareSimpleKeys(set1.SimpleKey, set2.SimpleKey);
    }
  }
}