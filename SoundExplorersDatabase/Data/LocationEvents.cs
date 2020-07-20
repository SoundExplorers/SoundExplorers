using System.Collections.Generic;
using VelocityDb;
using VelocityDb.Collection.BTree;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class LocationEvents : BTreeSet<Event> {
    internal LocationEvents(Location parent) {
      Parent = parent;
      ChildrenAdded = new List<Event>();
    }

    private List<Event> ChildrenAdded { get; }
    private Location Parent { get; }

    public new bool Add(Event child) {
      bool result = base.Add(child);
      if (result) {
        ChildrenAdded.Add(child);
      }
      return result;
    }

    public override ulong Persist(Placement place, SessionBase session,
      bool persistRefs = true,
      bool disableFlush = false, Queue<IOptimizedPersistable> toPersist = null) {
      ulong result = base.Persist(place, session, persistRefs, disableFlush, toPersist);
      if (ChildrenAdded.Count > 0) {
        foreach (var childAdded in ChildrenAdded) {
          childAdded.Location = Parent;
          session.Persist(childAdded);
        }
        ChildrenAdded.Clear();
      }
      return result;
      //return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }
  }
}