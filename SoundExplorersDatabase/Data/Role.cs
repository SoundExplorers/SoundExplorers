﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Role : EntityBase {
    public Role() : base(typeof(Role), nameof(Name), null) {
      Credits = new SortedChildList<Credit>(this);
    }

    [NotNull] public SortedChildList<Credit> Credits { get; }

    [CanBeNull]
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

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}