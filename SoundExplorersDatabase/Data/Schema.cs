using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Schema {
    private static Schema _instance;
    private IReadOnlyCollection<RelationInfo> _relations;

    [NotNull]
    public static Schema Instance => _instance ?? (_instance = new Schema());

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