using System.Collections.Generic;
using System.Data;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class NotablyNamedEntityList<TEntity> : EntityListBase<TEntity>
    where TEntity : EntityBase, INotablyNamed, new() {
    protected override TEntity CreateBackupEntity(TEntity entity) {
      return new TEntity {
        Name = entity.Name,
        Notes = entity.Notes
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(Location.Name), typeof(string)),
        new EntityColumn(nameof(Location.Notes), typeof(string))
      };
    }

    protected override IList<object> GetRowItemValuesFromEntity(TEntity entity) {
      return new List<object> {entity.Name, entity.Notes};
    }

    protected override void RestoreEntityPropertiesFromBackup(TEntity backupEntity,
      TEntity entityToRestore) {
      entityToRestore.Name = backupEntity.Name;
      entityToRestore.Notes = backupEntity.Notes;
    }

    protected override void UpdateEntityAtRow(DataRow row, TEntity entity) {
      var newName = row[nameof(Location.Name)].ToString();
      var newNotes = row[nameof(Location.Notes)].ToString();
      if (newName != entity.Name) {
        entity.Name = newName;
      }
      if (newNotes != entity.Notes) {
        entity.Notes = newNotes;
      }
    }
  }
}