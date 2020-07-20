using System.Collections.Generic;
using System.Linq;
using VelocityDb;
using VelocityDb.Collection.BTree;
using VelocityDb.Collection.Comparer;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class LocationEvents : BTreeSet<Event> {
    public LocationEvents(Location parent) {
      Parent = parent;
    }
    private Location Parent { get; }

    public new bool Add(Event child) {
      bool result = base.Add(child);
      if (result) {
        var reference = new Reference(this, "_events");
        child.References.AddFast(reference);
        child.Location = Parent;
      }
      return result;
    }

    public new bool Remove(Event child) {
      bool result = base.Remove(child);
      if (result) {
        child.References.Remove(
          child.References.First(r => r.To.Equals(this)));
        child.Location = null;
      }
      return result;
    }
  }
}