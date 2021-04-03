using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a Set's genre.
  /// </summary>
  [VelocityDb.Indexing.Index("_name")]
  public class Genre : EntityBase, INamedEntity {
    private string _name = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Genre() : base(typeof(Genre), nameof(Name), null) {
      Sets = new SortedEntityCollection<Set>();
    }

    public SortedEntityCollection<Set> Sets { get; }

    public string Name {
      get => _name;
      set {
        Update();
        _name = SimpleKey = value;
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Sets;
    }
  }
}