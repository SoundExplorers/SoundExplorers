using System;

namespace SoundExplorers.Data {
  public class Role : EntityBase, INamedEntity {
    /// <summary>
    ///   Role entity, usually representing a musical instrument.
    /// </summary>
    public Role() : base(typeof(Role), nameof(Name), null) {
      Credits = new SortedEntityCollection<Credit>();
    }

    public SortedEntityCollection<Credit> Credits { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Credits;
    }
  }
}