using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a type of Events.
  ///   A performance or rehearsal, for example.
  /// </summary>
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class EventType : EntityBase, INamedEntity {
    public const string DefaultName = "Performance";

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public EventType() : base(typeof(EventType), nameof(Name), null) {
      Events = new SortedEntityCollection<Event>();
    }

    public SortedEntityCollection<Event> Events { get; }

    public string Name {
      get => SimpleKey;
      set {
        Update();
        SimpleKey = value;
      }
    }

    public static EventType CreateDefault() {
      return new EventType {
        Name = DefaultName
      };
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Events;
    }
  }
}