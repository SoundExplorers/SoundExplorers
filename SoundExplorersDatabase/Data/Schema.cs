using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Schema {
    private static Schema _instance;
    private IEnumerable<RelationInfo> _relations;

    [NotNull]
    public static Schema Instance => _instance ?? (_instance = new Schema());

    [NotNull]
    public IEnumerable<RelationInfo> Relations =>
      _relations ?? (_relations = CreateRelations());

    [NotNull]
    protected virtual IEnumerable<RelationInfo> CreateRelations() {
      var list = new List<RelationInfo> {
        new RelationInfo(typeof(Act), typeof(Set), false),
        new RelationInfo(typeof(Event), typeof(Set), true),
        new RelationInfo(typeof(Location), typeof(Event), true),
        new RelationInfo(typeof(Newsletter), typeof(Event), false),
        new RelationInfo(typeof(Series), typeof(Event), false)
      };
      return list.ToArray();
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