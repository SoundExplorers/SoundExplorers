using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a series of Events.
  ///   A festival, for example.
  /// </summary>
  public class Series : EntityBase, INotablyNamedEntity {
    public const string DefaultName = "";
    private string _notes = null!;

    public Series() : base(typeof(Series), nameof(Name), null) {
      AllowBlankSimpleKey = true;
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

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public static Series CreateDefault() {
      return new Series {
        Name = DefaultName,
        Notes = "Required default"
      };
    }

    protected override IEnumerable GetChildren(Type childType) {
      return Events;
    }
  }
}