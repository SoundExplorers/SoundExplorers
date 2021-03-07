using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a location where Events have been held.
  ///   Typically a music venue, but could, for example,
  ///   be an outdoor location.
  /// </summary>
  public class Location : EntityBase, INotablyNamedEntity {
    private string _notes = null!;

    public Location() : base(typeof(Location), nameof(Name), null) {
      Events = new SortedChildList<Event>();
    }

    public SortedChildList<Event> Events { get; }

    public string Name {
      get => SimpleKey;
      set {
        //Debug.WriteLine($"Location.Name setting to '{value}'");
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override IEnumerable GetChildren(Type childType) {
      return Events;
    }
  }
}