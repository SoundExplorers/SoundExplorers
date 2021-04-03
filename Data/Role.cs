using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Role entity, usually representing a musical instrument.
  /// </summary>
  [VelocityDb.Indexing.Index("_name")]
  public class Role : EntityBase, INamedEntity {
    private string _name = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Role() : base(typeof(Role), nameof(Name), null) {
      Credits = new SortedEntityCollection<Credit>();
    }

    public SortedEntityCollection<Credit> Credits { get; }

    public string Name {
      get => _name;
      set {
        Update();
        _name = SimpleKey = value;
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Credits;
    }
  }
}