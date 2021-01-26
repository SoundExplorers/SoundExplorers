using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NotablyNamedEntityList<TEntity>
    : EntityListBase<TEntity, NotablyNamedBindingItem<TEntity>>
    where TEntity : EntityBase, INotablyNamedEntity, new() {
    protected override NotablyNamedBindingItem<TEntity> CreateBindingItem(
      TEntity entity) {
      return new() {
        Name = entity.Name, Notes = entity.Notes
      };
    }

    protected override BindingColumnList CreateColumns() {
      return new() {
        new BindingColumn(nameof(Location.Name)) {IsInKey = true},
        new BindingColumn(nameof(Location.Notes))
      };
    }
  }
}