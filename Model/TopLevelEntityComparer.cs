using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  /// <summary>
  ///   A comparer to be used when sorting an entity list containing top-level entities.
  /// </summary>
  public class TopLevelEntityComparer<TEntity> : Comparer<TEntity>
    where TEntity : IEntity {
    public override int Compare(TEntity? entity1, TEntity? entity2) {
      return Key.CompareSimpleKeys(entity1?.SimpleKey, entity2?.SimpleKey);
    }
  }
}