using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NamedEntityList<TEntity> : EntityListBase<TEntity, NamedBindingItem>
    where TEntity : INamedEntity {
    protected override NamedBindingItem CreateBindingItem(TEntity entity) {
      return new NamedBindingItem {Name = entity.Name};
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(INamedEntity.Name), typeof(string))
      };
    }
  }
}