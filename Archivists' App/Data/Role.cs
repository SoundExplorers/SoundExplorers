using System;
using System.Collections;

namespace SoundExplorers.Data {
  public class Role : EntityBase, INamedEntity {
    /// <summary>
    ///   Role entity, usually representing a musical instrument.
    /// </summary>
    public Role() : base(typeof(Role), nameof(Name), null) {
      Credits = new SortedChildList<Credit>();
    }

    public SortedChildList<Credit> Credits { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Credits;
    }
  }
}