using System.Collections.Generic;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  public class TestSchema : Schema {
    protected override IList<RelationInfo> CreateRelations() {
      return new List<RelationInfo> {
        new RelationInfo(typeof(Father), typeof(Daughter), false),
        new RelationInfo(typeof(Father), typeof(Son), false),
        new RelationInfo(typeof(Mother), typeof(Daughter), true),
        new RelationInfo(typeof(Mother), typeof(Son), false)
      };
    }
  }
}