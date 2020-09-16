using System.Collections.Generic;

namespace SoundExplorers.Data {
  public class EntityComparer<TEntity> : Comparer<TEntity> where TEntity : IEntity {
    public override int Compare(TEntity x, TEntity y) {
      if (x?.Key < y?.Key) {
        return -1;
      }
      return x?.Key == y?.Key ? 0 : 1;
    }
  }
}