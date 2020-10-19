﻿using System;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class
    NotablyNamedEntityList<TEntity> : EntityListBase<TEntity, NotablyNamedBindingItem>
    where TEntity : INotablyNamedEntity, new() {
    protected override TEntity CreateBackupEntity(
      TEntity entity) {
      return new TEntity {
        Name = entity.Name,
        Notes = entity.Notes
      };
    }

    protected override NotablyNamedBindingItem CreateBindingItem(TEntity entity) {
      return new NotablyNamedBindingItem {Name = entity.Name, Notes = entity.Notes};
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Location.Name)),
        new EntityColumn(nameof(Location.Notes))
      };
    }

    protected override TEntity CreateEntity(NotablyNamedBindingItem bindingItem) {
      return new TEntity {
        Name = bindingItem.Name,
        Notes = bindingItem.Notes
      };
    }

    protected override void RestoreEntityPropertiesFromBackup(TEntity backupEntity,
      TEntity entityToRestore) {
      entityToRestore.Name = backupEntity.Name;
      entityToRestore.Notes = backupEntity.Notes;
    }

    protected override void UpdateEntityProperty(string propertyName, object newValue,
      TEntity entity) {
      switch (propertyName) {
        case nameof(entity.Name):
          entity.Name = newValue?.ToString();
          break;
        case nameof(entity.Notes):
          entity.Notes = newValue?.ToString();
          break;
        default:
          throw new ArgumentException(
            $"{nameof(propertyName)} '{propertyName}' is not supported.",
            nameof(propertyName));
      }
    }
  }
}