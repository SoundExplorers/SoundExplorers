using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public static class Schema {
    //private static bool _existsOnDatabase;
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

    public static bool ExistsOnDatabase([NotNull] SessionBase session) {
      return session.ContainsDatabase(session.DatabaseLocations.First(), 1);
      // TODO Fails due to multi-threaded tests? 
      // bool result;
      // if (_existsOnDatabase) {
      //   result = true;
      // } else {
      //   result = session.ContainsDatabase(session.DatabaseLocations.First(), 1);
      //   _existsOnDatabase = result;
      // }
      // return result;
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