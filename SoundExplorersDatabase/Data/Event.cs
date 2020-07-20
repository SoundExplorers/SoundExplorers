using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Collection.BTree.Extensions;
using VelocityDb.Indexing;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  [Index("_date, _location")]
  [UniqueConstraint]
  public class Event : ReferenceTracked {
    private DateTime _date;
    private Location _location;
    private string _notes;

    public Event(DateTime date, [NotNull] Location location) {
      Date = date;
      Location = location;
    }

    [PublicAPI]
    public DateTime Date {
      get => _date;
      set {
        Update();
        _date = value;
      }
    }

    public Location Location {
      get => _location;
      set {
        if (value.Equals(_location)) {
          return;
        }
        IsLocationChanging = true;
        LocationToMoveFrom = _location;
        Update();
        _location = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        Update();
        _notes = value;
      }
    }

    private bool IsLocationChanging { get; set; }
    private Location LocationToMoveFrom { get; set; }

    public override ulong Persist(Placement place, SessionBase session, bool persistRefs = true,
      bool disableFlush = false, Queue<IOptimizedPersistable> toPersist = null) {
      ulong result = base.Persist(place, session, persistRefs, disableFlush, toPersist);
      if (IsLocationChanging) {
        LocationToMoveFrom?.Events.Remove(this);
        Location?.Events.Add(this);
        IsLocationChanging = false;
        LocationToMoveFrom = null;
      }
      return result;
    }

    [NotNull]
    public static Event Read(DateTime date, [NotNull] Location location,
      [NotNull] SessionBase session) {
      // ReSharper disable once ReplaceWithSingleCallToFirst
      return session.Index<Event>()
        .Where(@event => @event.Date == date
                         && @event.Location.Name == location.Name).First();
    }
  }
}