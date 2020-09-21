using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NamedEntityList<TEntity> : EntityListBase<TEntity, NamedBindingItem<TEntity>>
    where TEntity : EntityBase, INamed, new() {
    protected override NamedBindingItem<TEntity> CreateBindingItem(TEntity entity) {
      return new NamedBindingItem<TEntity> {Name = entity.Name};
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(INamed.Name), typeof(string))
      };
    }
  }
}