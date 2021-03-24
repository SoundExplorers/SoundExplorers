﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class TestSchema : Schema {
    private static TestSchema? _instance;

    public new static TestSchema Instance =>
      _instance ??= new TestSchema();

    protected override IEnumerable<RelationInfo> CreateRelations() {
      var list = new List<RelationInfo> {
        new RelationInfo(typeof(Father), typeof(Daughter), false),
        new RelationInfo(typeof(Father), typeof(Son), false),
        new RelationInfo(typeof(Mother), typeof(Daughter), true),
        new RelationInfo(typeof(Mother), typeof(Son), false)
      };
      return new ReadOnlyCollection<RelationInfo>(list);
    }
  }
}