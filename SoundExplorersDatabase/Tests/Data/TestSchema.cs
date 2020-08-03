using System.Collections.Generic;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  public static class TestSchema {
    private static IList<RelationInfo> _relations;

    [NotNull]
    public static IList<RelationInfo> Relations =>
      _relations ?? (_relations = CreateRelations());

    [NotNull]
    private static IList<RelationInfo> CreateRelations() {
      return new List<RelationInfo> {
        new RelationInfo(typeof(Father), typeof(Daughter), false),
        new RelationInfo(typeof(Father), typeof(Son), false),
        new RelationInfo(typeof(Mother), typeof(Daughter), true),
        new RelationInfo(typeof(Mother), typeof(Son), false)
      };
    }
  }
}