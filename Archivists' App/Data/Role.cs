﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  public class Role : EntityBase {
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

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      throw new NotSupportedException();
    }
  }
}