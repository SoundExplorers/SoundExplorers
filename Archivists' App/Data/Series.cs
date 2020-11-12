using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a series of Events.
  ///   A festival, for example.
  /// </summary>
  public class Series : EntityBase, INotablyNamedEntity {
    private string _notes;

    public Series() : base(typeof(Series), nameof(Name), null) {
      Events = new SortedChildList<Event>();
    }

    [NotNull] public SortedChildList<Event> Events { get; }

    [CanBeNull]
    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    [CanBeNull]
    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}