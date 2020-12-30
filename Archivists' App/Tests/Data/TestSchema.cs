using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class TestSchema : Schema {
    private static TestSchema? _instance;

    [NotNull]
    public new static TestSchema Instance =>
      _instance ??= new TestSchema();

    protected override IEnumerable<RelationInfo> CreateRelations() {
      var list = new List<RelationInfo> {
        new(typeof(Father), typeof(Daughter), false),
        new(typeof(Father), typeof(Son), false),
        new(typeof(Mother), typeof(Daughter), true),
        new(typeof(Mother), typeof(Son), false)
      };
      return new ReadOnlyCollection<RelationInfo>(list);
    }
  }
}