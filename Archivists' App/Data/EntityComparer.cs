using System.Collections.Generic;

namespace SoundExplorers.Data {
  public class EntityComparer<TEntity> : Comparer<TEntity> where TEntity : IEntity {
    public override int Compare(TEntity? x, TEntity? y) {
      return new KeyComparer().Compare(x?.Key, y?.Key);
    }
  }
}