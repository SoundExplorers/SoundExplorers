using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a type of Events.
  ///   A performance or rehearsal, for example.
  /// </summary>
  public class EventType : EntityBase, INamedEntity {
    public const string DefaultName = "Performance";

    public EventType() : base(typeof(EventType), nameof(Name), null) {
      Events = new SortedChildList<Event>();
    }

    public SortedChildList<Event> Events { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    public static EventType CreateDefault() {
      return new EventType {
        Name = DefaultName
      };
    }

    protected override IEnumerable GetChildren(Type childType) {
      return Events;
    }
  }
}