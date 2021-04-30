using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a Set's genre.
  /// </summary>
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class Genre : EntityBase, INamedEntity {
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Genre() : base(typeof(Genre), nameof(Name), null) {
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

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Sets;
    }
  }
}