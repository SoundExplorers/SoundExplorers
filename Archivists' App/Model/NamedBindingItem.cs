using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class NamedBindingItem<TEntity> : EntityBindingItemBase<TEntity>
    where TEntity : IEntity, INamed, new() {
    public string Name { get; set; }

    public override TEntity CreateEntity() {
      return new TEntity {Name = Name};
    }
  }
}