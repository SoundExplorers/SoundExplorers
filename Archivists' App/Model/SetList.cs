using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetList : EntityListBase<Set> {
    public SetList() : base(typeof(EventList)) { }

    public override IList GetChildren(int rowIndex) {
      return (IList)this[rowIndex].Pieces.Values;
    }

    protected override Set CreateBackupEntity(Set location) {
      return new Set {
        SetNo = location.SetNo, Act = location.Act, Genre = location.Genre,
        Notes = location.Notes
      };
    }

    protected override EntityColumnList CreateColumns() {
      var result = new EntityColumnList {
        new EntityColumn(nameof(Set.SetNo), typeof(int)),
        new EntityColumn(nameof(Set.Act), typeof(string),
          typeof(Act), nameof(Act.Name)),
        new EntityColumn(nameof(Set.Genre), typeof(string),
          typeof(Genre), nameof(Genre.Name)),
        new EntityColumn(nameof(Set.Notes), typeof(string))
      };
      if (IsParentList) {
        result.Insert(0,
          new EntityColumn(nameof(Event.Date), typeof(DateTime)));
        result.Insert(1,
          new EntityColumn(nameof(Event.Location), typeof(string)));
      }
      return result;
    }

    protected override IList<object> GetRowItemValuesFromEntity(Set set) {
      var result = new List<object> {set.SetNo, set.Act?.Name, set.Genre.Name, set.Notes};
      if (IsParentList) {
        result.Insert(0, set.Event.Date);
        result.Insert(1, set.Event.Location.Name);
      }
      return result;
    }

    protected override void RestoreEntityPropertiesFromBackup(Set backupSet,
      Set setToRestore) {
      setToRestore.SetNo = backupSet.SetNo;
      setToRestore.Act = backupSet.Act;
      setToRestore.Genre = backupSet.Genre;
      setToRestore.Notes = backupSet.Notes;
    }

    protected override void UpdateEntityAtRow(DataRow row, Set set) {
      var newSetNo = (int)row[nameof(Set.SetNo)];
      var newActName = row[nameof(Set.Act)].ToString();
      var newGenreName = row[nameof(Set.Genre)].ToString();
      var newNotes = row[nameof(Set.Notes)].ToString();
      if (newSetNo != set.SetNo) {
        set.SetNo = newSetNo;
      }
      if (newActName != set.Act?.Name) {
        set.Act = !string.IsNullOrEmpty(newActName)
          ? QueryHelper.Read<Act>(newActName, Session)
          : null;
      }
      if (newGenreName != set.Genre.Name) {
        set.Genre = QueryHelper.Read<Genre>(newGenreName, Session);
      }
      if (newNotes != set.Notes) {
        set.Notes = newNotes;
      }
    }
  }
}