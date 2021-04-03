using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an act that has performed at Events.
  /// </summary>
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class Act : EntityBase, INotablyNamedEntity {
    public const string DefaultName = "";
    private string _notes = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Act() : base(typeof(Act), nameof(Name), null) {
      AllowBlankSimpleKey = true;
      Sets = new SortedEntityCollection<Set>();
    }

    public SortedEntityCollection<Set> Sets { get; }

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

    public static Act CreateDefault() {
      return new Act {
        Name = DefaultName,
        Notes = "Required default"
      };
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Sets;
    }
  }
}