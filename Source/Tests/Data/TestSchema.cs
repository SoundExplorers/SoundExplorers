using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;
using VelocityDb.Collection.BTree;
using VelocityDb.Collection.Comparer;

namespace SoundExplorers.Tests.Data {
  public class TestSchema : Schema {
    private static TestSchema? _instance;

    public new static TestSchema Instance =>
      _instance ??= new TestSchema();

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<Type> CreatePersistableTypes() {
      var list = new List<Type> {
        typeof(Daughter),
        typeof(BTreeSet<Daughter>),
        typeof(CompareByFieldIndex<Daughter>),
        typeof(Father),
        typeof(BTreeSet<Father>),
        typeof(CompareByFieldIndex<Father>),
        typeof(Mother),
        typeof(BTreeSet<Mother>),
        typeof(CompareByFieldIndex<Mother>),
        typeof(Son),
        typeof(BTreeSet<Son>),
        typeof(CompareByFieldIndex<Son>)
      };
      return list.ToArray();
    }

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