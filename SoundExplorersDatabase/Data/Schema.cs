using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Schema {
    private static Schema _instance;
    private bool _existsOnDatabase;
    private IReadOnlyCollection<RelationInfo> _relations;

    internal static Schema Instance {
      get => _instance ?? (_instance = new Schema());
      set => _instance = value;
    }

    [NotNull]
    public IReadOnlyCollection<RelationInfo> Relations {
      get => _relations ?? (_relations = CreateRelations());
      internal set => _relations = value;
    }

    [NotNull]
    protected virtual ReadOnlyCollection<RelationInfo> CreateRelations() {
      var list = new List<RelationInfo> {
        new RelationInfo(typeof(Location), typeof(Event), true),
        new RelationInfo(typeof(Newsletter), typeof(Event), false),
        new RelationInfo(typeof(Series), typeof(Event), false)
      };
      return new ReadOnlyCollection<RelationInfo>(list);
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