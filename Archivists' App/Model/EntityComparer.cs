using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EntityComparer<TEntity> : Comparer<TEntity> where TEntity : IEntity {
    public override int Compare(TEntity? entity1, TEntity? entity2) {
      return new SimpleKeyComparer().Compare(entity1?.SimpleKey, entity2?.SimpleKey);
    }
  }
}