using System;
using VelocityDb;
using VelocityDb.Collection.BTree;
using VelocityDb.Indexing;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Newsletter : ReferenceTracked {

    [Index]
    [UniqueConstraint]
    private DateTime _date;

    [Index]
    [UniqueConstraint]
    private string _path;

    BTreeSet<Event> _events;

    [FieldAccessor("_date")]
    public DateTime Date {
      get {
        return _date;
      }
      set {
        Update();
        _date = value;
      }
    }

    [FieldAccessor("_path")]
    public string Path {
      get {
        return _path;
      }
      set {
        Update();
        _path = value;
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

  }
}
