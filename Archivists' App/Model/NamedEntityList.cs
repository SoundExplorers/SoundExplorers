using System;
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
        new EntityColumn(nameof(INamedEntity.Name))
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

    protected override void UpdateEntityProperty(string propertyName, object newValue,
      TEntity entity) {
      switch (propertyName) {
        case nameof(entity.Name):
          entity.Name = newValue?.ToString();
          break;
        default:
          throw new ArgumentException(
            $"{nameof(propertyName)} '{propertyName}' is not supported.",
            nameof(propertyName));
      }
    }
  }
}