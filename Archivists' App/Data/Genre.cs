using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      throw new NotSupportedException();
    }
  }
}