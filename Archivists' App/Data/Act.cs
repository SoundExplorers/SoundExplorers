using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an act that has performed at Events.
  /// </summary>
  public class Act : EntityBase, INotablyNamedEntity {
    public const string DefaultName = "";
    private string _notes = null!;

    public Act() : base(typeof(Act), nameof(Name), null) {
      AllowBlankSimpleKey = true;
      Sets = new SortedChildList<Set>();
    }

    public SortedChildList<Set> Sets { get; }

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

    public static Act CreateDefault() {
      return new Act {
        Name = DefaultName,
        Notes = "Required default"
      };
    }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }
  }
}