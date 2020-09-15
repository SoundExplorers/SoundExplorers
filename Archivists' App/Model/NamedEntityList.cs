using System.Collections.Generic;
using System.Data;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public abstract class NamedEntityList<TEntity> : EntityListBase<TEntity>
    where TEntity : EntityBase, INamed, new() {
    protected override TEntity CreateBackupEntity(TEntity entity) {
      return new TEntity {
        Name = entity.Name
      };
    }

    protected override EntityColumnList CreateColumns() {
      return new EntityColumnList {
        new EntityColumn(nameof(INamed.Name), typeof(string))
      };
    }

    protected override IList<object> GetRowItemValuesFromEntity(TEntity entity) {
      return new List<object> {entity.Name};
    }

    protected override void RestoreEntityPropertiesFromBackup(TEntity backupEntity,
      TEntity entityToRestore) {
      entityToRestore.Name = backupEntity.Name;
    }

    protected override void UpdateEntityAtRow(DataRow row, TEntity entity) {
      var newName = row[nameof(Location.Name)].ToString();
      if (newName != entity.Name) {
        entity.Name = newName;
      }
    }
  }
}