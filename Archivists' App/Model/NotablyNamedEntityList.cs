using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class NotablyNamedEntityList<TEntity> : EntityListBase<TEntity, NotablyNamedBindingItem>
    where TEntity : INotablyNamedEntity {
    protected override NotablyNamedBindingItem CreateBindingItem(TEntity entity) {
      return new NotablyNamedBindingItem {Name = entity.Name, Notes = entity.Notes};
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Location.Name), typeof(string)),
        new EntityColumn(nameof(Location.Notes), typeof(string))
      };
    }
  }
}