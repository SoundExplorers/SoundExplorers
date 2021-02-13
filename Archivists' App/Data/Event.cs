using System;
using System.Collections;
using System.Collections.Generic;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an event.
  ///   Usually a performance, but could, for example,
  ///   be a rehearsal.
  /// </summary>
  public class Event : EntityBase {
    public const string DefaultEventTypeName = "Performance";
    public const string DefaultSeriesName = "";
    private DateTime _date;
    private EventType? _eventType;
    private Newsletter? _newsletter;
    private string _notes = null!;
    private Series? _series;

    public Event() : base(typeof(Event), nameof(Date), typeof(Location)) {
      _date = DefaultDate;
      Sets = new SortedChildList<Set>();
    }

    public DateTime Date {
      get => _date;
      set {
        if (value <= DefaultDate) {
          throw new PropertyConstraintException(
            $"Event Date must be later than {DateToSimpleKey(DefaultDate)}.",
            nameof(Date));
        }
        UpdateNonIndexField();
        _date = value.Date;
        SimpleKey = DateToSimpleKey(_date);
      }
    }

    public EventType EventType {
      get => _eventType!;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(EventType), value);
        _eventType = value;
      }
    }

    public Location Location {
      get => (Location)IdentifyingParent!;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public Newsletter Newsletter {
      get => _newsletter!;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Newsletter), value);
        _newsletter = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public Series Series {
      get => _series!;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Series), value);
        _series = value;
      }
    }

    public SortedChildList<Set> Sets { get; }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }

    public override ulong Persist(Placement place, SessionBase session, bool persistRefs = true,
      bool disableFlush = false, Queue<IOptimizedPersistable>? toPersist = null) {
      if (_eventType == null) {
        EventType = QueryHelper.Read<EventType>(DefaultEventTypeName, session);
      }
      if (_newsletter == null) {
        Newsletter = QueryHelper.Read<Newsletter>(DateToSimpleKey(DefaultDate), session);
      }
      if (_series == null) {
        Series = QueryHelper.Read<Series>(DefaultSeriesName, session);
      }
      return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      if (parentEntityType == typeof(EventType)) {
        _eventType = newParent as EventType;
      } else if (parentEntityType == typeof(Newsletter)) {
        _newsletter = newParent as Newsletter;
      } else {
        _series = newParent as Series;
      }
    }
  }
}