using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a type of Events.
  ///   A performance or rehearsal, for example.
  /// </summary>
  [VelocityDb.Indexing.Index("_name")]
  public class EventType : EntityBase, INamedEntity {
    public const string DefaultName = "Performance";
    private string _name = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public EventType() : base(typeof(EventType), nameof(Name), null) {
      Events = new SortedEntityCollection<Event>();
    }

    public SortedEntityCollection<Event> Events { get; }

    public string Name {
      get => _name;
      set {
        Update();
        _name = SimpleKey = value;
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