using System;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NamedEntityList<TEntity> : EntityListBase<TEntity, NamedBindingItem>
    where TEntity : EntityBase, INamedEntity, new() {

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

    protected override TEntity CreateEntity(NamedBindingItem bindingItem) {
      return new TEntity {
        Name = bindingItem.Name
      };
    }

    protected override void RestoreEntityPropertiesFromBackup(TEntity backupEntity,
      TEntity entityToRestore) {
      entityToRestore.Name = backupEntity.Name;
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