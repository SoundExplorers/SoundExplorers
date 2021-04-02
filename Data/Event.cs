using System;
using System.Diagnostics.CodeAnalysis;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an event.
  ///   Usually a performance, but could, for example,
  ///   be a rehearsal.
  /// </summary>
  public class Event : EntityBase {
    private DateTime _date;
    private EventType? _eventType;
    private Newsletter? _newsletter;
    private string _notes = null!;
    private Series? _series;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Event() : base(typeof(Event), nameof(Date),
      typeof(Location)) {
      _date = DefaultDate;
      Sets = new SortedEntityCollection<Set>();
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

    public SortedEntityCollection<Set> Sets { get; }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Sets;
    }

    protected override void CheckCanPersist(SessionBase session) {
      if (_eventType == null) {
        EventType = QueryHelper.Read<EventType>(EventType.DefaultName, session);
      }
      if (_newsletter == null) {
        Newsletter = QueryHelper.Read<Newsletter>(DateToSimpleKey(DefaultDate), session);
      }
      if (_series == null) {
        Series = QueryHelper.Read<Series>(Series.DefaultName, session);
      }
      base.CheckCanPersist(session);
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