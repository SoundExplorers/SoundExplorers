using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing an act that has performed at Events.
  /// </summary>
  public class Act : EntityBase, INotablyNamedEntity {
    public const string DefaultName = "";
    private string _notes = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Act(SortedEntityCollection<Act> root) : base(
      root, typeof(Act), nameof(Name), null) {
      AllowBlankSimpleKey = true;
      Sets = new SortedEntityCollection<Set>();
    }

    public SortedEntityCollection<Set> Sets { get; }

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

    public static Act CreateDefault(SortedEntityCollection<Act> root) {
      return new Act(root) {
        Name = DefaultName,
        Notes = "Required default"
      };
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Sets;
    }
  }
}