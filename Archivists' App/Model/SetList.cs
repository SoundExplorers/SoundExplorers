using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetList : EntityListBase<Set, SetBindingItem> {
    public SetList() : base(typeof(EventList)) { }

    public override IList GetChildrenForMainList(int rowIndex) {
      return this[rowIndex].Pieces.Values.ToList();
    }

    protected override SetBindingItem CreateBindingItem(Set set) {
      return new() {
        Date = set.Event.Date, Location = set.Event.Location.Name!,
        SetNo = set.SetNo,
        Act = set.Act?.Name,
        Genre = set.Genre.Name,
        Notes = set.Notes
      };
    }

    protected override TypedBindingList<Set, SetBindingItem> CreateBindingList(
      IList<SetBindingItem> list) {
      return new SetBindingList(list);
    }

    protected override BindingColumnList CreateColumns() {
      var result = new BindingColumnList {
        new(nameof(Event.Date), typeof(DateTime)) {
          IsVisible = IsParentList
        },
        new(nameof(Event.Location), typeof(string)) {
          IsVisible = IsParentList
        },
        new(nameof(Set.SetNo), typeof(int)) {IsInKey = true},
        new(nameof(Set.Act), typeof(string),
          typeof(ActList), nameof(Act.Name)),
        new(nameof(Set.Genre), typeof(string),
          typeof(GenreList), nameof(Genre.Name)),
        new(nameof(Set.Notes), typeof(string))
      };
      return result;
    }
  }
}