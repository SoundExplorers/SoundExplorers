using VelocityDb.Collection.BTree;
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
        var reference = new Reference(child, "_events");
        Parent.References.AddFast(reference);
        if (!child.IsChangingLocation) {
          child.Location = Parent;
        }
      }
      return result;
    }

    // public new bool Remove(Event child) {
    //   //Debug.WriteLine($"LocationEvents.Remove: From {Count} Events");
    //   Parent.Update(); // ??
    //   Update(); // ??
    //   // Parent.References.Remove(
    //   //   Parent.References.First(r => r.To.Equals(child)));
    //   bool result = base.Remove(child);
    //   //Debug.WriteLine($"LocationEvents.Remove: To = {Count} Events");
    //   if (result) {
    //     Parent.References.Remove(
    //       Parent.References.First(r => r.To.Equals(child)));
    //     if (!child.IsChangingLocation) {
    //       child.Location = null;
    //     }
    //   }
    //   return result;
    // }
  }
}