using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Collection.BTree;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Location : ReferenceTracked {

    [Index]
    [UniqueConstraint]
    private string _name;

    private string _notes;

    BTreeSet<Event> _events;

    [FieldAccessor("_name")]
    public string Name {
      get {
        return _name;
      }
      set {
        Update();
        _name = value;
      }
    }

    public string Notes {
      get {
        return _notes;
      }
      set {
        Update();
        _notes = value;
      }
    }

    public BTreeSet<Event> Events {
      get {
        return _events;
      }
      set {
        Update();
        _events = value;
      }
    }

    public void AddEvent(Event @event) {
      if (Events == null) {
        Events = new BTreeSet<Event>();
      }
      Events.Add(@event);
      //if (Events.Add(@event)) {
      //  var reference = new Reference(_events, "_events");
      //  @event.References.AddFast(reference);
      //}
    }

    [NotNull] 
    public static Location Read([NotNull] string name, [NotNull] SessionBase session) {
      return (
        from Location location in session.Index<Location>("_name")
        where location.Name == name
        select location).First();
    }
  }
}
