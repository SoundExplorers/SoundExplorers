using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class EntityBindingItemBase<TEntity> where TEntity : IEntity {
    public abstract TEntity CreateEntity();
  }
}