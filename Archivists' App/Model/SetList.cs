using System;
using System.Collections.Generic;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetList : EntityListBase<Set> {
    public SetList(bool isParentList = false) : base(isParentList, typeof(EventList)) { }

    protected override Set CreateBackupEntity(Set set) {
      return new Set
        {SetNo = set.SetNo, Act = set.Act, Genre = set.Genre, Notes = set.Notes};
    }

    protected override EntityColumnList CreateColumns() {
      var result = new EntityColumnList {
        new EntityColumn(nameof(Set.SetNo), typeof(int)),
        new EntityColumn(nameof(Set.Act), typeof(string),
          nameof(Act), nameof(Act.Name)),
        new EntityColumn(nameof(Set.Genre), typeof(string),
          nameof(Genre), nameof(Genre.Name)),
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

    protected override void UpdateEntityAtRow(int rowIndex) {
      var row = Table.Rows[rowIndex];
      var newSetNo = (int)row[nameof(Set.SetNo)];
      var newActName = row[nameof(Set.Act)].ToString();
      var newGenreName = row[nameof(Set.Genre)].ToString();
      var newNotes = row[nameof(Set.Notes)].ToString();
      var set = this[rowIndex];
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