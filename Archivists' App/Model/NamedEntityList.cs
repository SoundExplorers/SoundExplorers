using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NamedEntityList<TEntity> : EntityListBase<TEntity, NamedBindingItem<TEntity>>
    where TEntity : EntityBase, INamedEntity, new() {
    protected override NamedBindingItem<TEntity> CreateBindingItem(TEntity entity) {
      return new() {Name = entity.Name};
    }

    protected override BindingColumnList CreateColumns() {
      return new() {
        new BindingColumn(nameof(INamedEntity.Name), typeof(string)) {IsInKey = true}
      };
    }
  }
}