using System;
using System.Collections;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a Set's genre.
  /// </summary>
  public class Genre : EntityBase, INamedEntity {
    public Genre() : base(typeof(Genre), nameof(Name), null) {
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

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
    }
  }
}