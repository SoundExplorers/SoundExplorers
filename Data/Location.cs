using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a location where Events have been held.
  ///   Typically a music venue, but could, for example,
  ///   be an outdoor location.
  /// </summary>
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class Location : EntityBase, INotablyNamedEntity {
    private string _notes = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Location() : base(typeof(Location), nameof(Name), null) {
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

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Events;
    }
  }
}