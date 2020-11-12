using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a location where Events have been held.
  ///   Typically a music venue, but could, for example,
  ///   be an outdoor location.
  /// </summary>
  public class Location : EntityBase, INotablyNamedEntity {
    private string _notes;

    public Location() : base(typeof(Location), nameof(Name), null) {
      Events = new SortedChildList<Event>();
    }

    [NotNull] public SortedChildList<Event> Events { get; }

    [CanBeNull]
    public string Name {
      get => SimpleKey;
      set {
        //Debug.WriteLine($"Location.Name setting to '{value}'");
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