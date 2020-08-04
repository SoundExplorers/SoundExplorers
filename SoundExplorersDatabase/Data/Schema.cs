using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Schema {
    private bool _existsOnDatabase;
    private static Schema _instance;
    private IList<RelationInfo> _relations;

    internal static Schema Instance {
      get => _instance ?? (_instance = new Schema());
      set => _instance = value;
    }

    [NotNull]
    public IList<RelationInfo> Relations {
      get => _relations ?? (_relations = CreateRelations());
      internal set => _relations = value;
    }

    [NotNull]
    protected virtual IList<RelationInfo> CreateRelations() {
      return new List<RelationInfo>();
    }

    public bool ExistsOnDatabase([NotNull] SessionBase session) {
      bool result;
      if (_existsOnDatabase) {
        result = true;
      } else {
        result = session.ContainsDatabase(session.DatabaseLocations.First(), 1);
        _existsOnDatabase = result;
      }
      return result;
    }

    [CanBeNull]
    public RelationInfo FindRelation(Type parentType, Type childType) {
      return (
        from relation in Relations
        where relation.ParentType == parentType &&
              relation.ChildType == childType
        select relation).FirstOrDefault();
    }
  }
}