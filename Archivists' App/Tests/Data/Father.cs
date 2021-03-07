﻿using System;
using System.Collections;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Father : EntityBase {
    public Father(QueryHelper queryHelper) : base(
      typeof(Father),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>();
      Sons = new SortedChildList<Son>();
    }

    public SortedChildList<Daughter> Daughters { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    public SortedChildList<Son> Sons { get; }

    protected override IEnumerable GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }
  }
}