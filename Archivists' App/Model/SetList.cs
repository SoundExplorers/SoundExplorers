using System;
using System.Collections.Generic;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetList : EntityListBase<Set, SetBindingItem> {
    public SetList() : base(typeof(EventList)) { }

    protected override SetBindingItem CreateBindingItem(Set set) {
      return new() {
        Date = set.Event.Date, Location = set.Event.Location.Name!,
        SetNo = set.SetNo,
        Act = set.Act?.Name,
        Genre = set.Genre.Name,
        IsPublic = set.IsPublic,
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
          IsVisible = ParentListType == null
        },
        new(nameof(Event.Location), typeof(string)) {
          IsVisible = ParentListType == null
        },
        new(nameof(Set.SetNo), typeof(int)) {IsInKey = true},
        new(nameof(Set.Act), typeof(string),
          new ReferenceType(typeof(ActList), nameof(Act.Name))),
        new(nameof(Set.Genre), typeof(string),
          new ReferenceType(typeof(GenreList), nameof(Genre.Name))),
        new(nameof(Set.IsPublic), typeof(bool)) {DisplayName = "Public?"},
        new(nameof(Set.Notes), typeof(string))
      };
      return result;
    }
 
    public override IdentifyingParentChildren GetIdentifyingParentChildrenForMainList(
      int rowIndex) {
      return new(this[rowIndex], this[rowIndex].Pieces.Values.ToList());
    }
  }
}