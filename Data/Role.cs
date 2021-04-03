using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Role entity, usually representing a musical instrument.
  /// </summary>
  [VelocityDb.Indexing.Index("_simpleKey")]
  public class Role : EntityBase, INamedEntity {

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Role() : base(typeof(Role), nameof(Name), null) {
      Credits = new SortedEntityCollection<Credit>();
    }

    public SortedEntityCollection<Credit> Credits { get; }

    public string Name {
      get => SimpleKey;
      set {
        Update();
        SimpleKey = value;
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Credits;
    }
  }
}