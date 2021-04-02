using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class TestSchema : Schema {
    private static TestSchema? _instance;

    public new static TestSchema Instance =>
      _instance ??= new TestSchema();

    [ExcludeFromCodeCoverage]
    protected override IEnumerable<Type> CreatePersistableTypes() {
      var list = new List<Type> {
        typeof(Daughter),
        typeof(SortedEntityCollection<Daughter>),
        typeof(Father),
        typeof(SortedEntityCollection<Father>),
        typeof(Mother),
        typeof(SortedEntityCollection<Mother>),
        typeof(Son),
        typeof(SortedEntityCollection<Son>)
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

    protected override IDictionary<Type, Type> CreateRootTypes() {
      return new Dictionary<Type, Type> {
        [typeof(Daughter)] = typeof(SortedEntityCollection<Daughter>),
        [typeof(Father)] = typeof(SortedEntityCollection<Father>),
        [typeof(Mother)] = typeof(SortedEntityCollection<Mother>),
        [typeof(Son)] = typeof(SortedEntityCollection<Son>)
      };
    }
  }
}