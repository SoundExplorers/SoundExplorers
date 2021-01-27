using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a type of Events.
  ///   A performance or rehearsal, for example.
  /// </summary>
  public class EventType : EntityBase, INamedEntity {
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

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      throw new NotSupportedException();
    }
  }
}