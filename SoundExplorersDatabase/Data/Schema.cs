using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public static class Schema {
    private static IList<RelationInfo> _relations;

    [NotNull]
    public static IList<RelationInfo> Relations {
      get => _relations ?? (_relations = CreateRelations());
      internal set => _relations = value;
    }

    [NotNull]
    private static IList<RelationInfo> CreateRelations() {
      return new List<RelationInfo>();
    }

    [CanBeNull]
    public static RelationInfo FindRelation(Type parentType, Type childType) {
      return (
        from relation in Relations
        where relation.ParentType == parentType &&
              relation.ChildType == childType
        select relation).FirstOrDefault();
    }
  }
}