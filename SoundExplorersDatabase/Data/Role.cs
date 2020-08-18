using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Role : EntityBase {
    public Role() : base(typeof(Role), nameof(Name), null) {
      //Credits = new SortedChildList<Credit>(this);
    }

    [CanBeNull]
    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    //[NotNull] public SortedChildList<Credit> Credits { get; }

    protected override IDictionary GetChildren(Type childType) {
      //return Credits;
      throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}