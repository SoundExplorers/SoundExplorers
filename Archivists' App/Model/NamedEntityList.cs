using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NamedEntityList<TEntity> : EntityListBase<TEntity, NamedBindingItem>
    where TEntity : INamedEntity, new() {
    protected override NamedBindingItem CreateBackupBindingItem(
      NamedBindingItem bindingItem) {
      return new NamedBindingItem {
        Name = bindingItem.Name
      };
    }

    protected override TEntity CreateBackupEntity(
      TEntity entity) {
      return new TEntity {
        Name = entity.Name
      };
    }

    protected override NamedBindingItem CreateBindingItem(TEntity entity) {
      return new NamedBindingItem {Name = entity.Name};
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(INamedEntity.Name), typeof(string))
      };
    }

    protected override void RestoreBindingItemPropertiesFromBackup(
      NamedBindingItem backupBindingItem,
      NamedBindingItem bindingItemToRestore) {
      bindingItemToRestore.Name = backupBindingItem.Name;
    }

    protected override void RestoreEntityPropertiesFromBackup(TEntity backupEntity,
      TEntity entityToRestore) {
      entityToRestore.Name = backupEntity.Name;
    }

    protected override void UpdateEntity(NamedBindingItem bindingItem, TEntity entity) {
      entity.Name = bindingItem.Name;
    }
  }
}