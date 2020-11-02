using System.Collections;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SetList : EntityListBase<Set, SetBindingItem> {
    public SetList() : base(typeof(EventList)) { }

    public override IList GetChildrenForMainList(int rowIndex) {
      return this[rowIndex].Pieces.Values.ToList();
    }

    protected override SetBindingItem CreateBindingItem(Set set) {
      return new SetBindingItem {
        Date = set.Event.Date, Location = set.Event.Location.Name,
        SetNo = set.SetNo, Act = set.Act?.Name,
        Genre = set.Genre.Name,
        Notes = set.Notes
      };
    }

    protected override BindingColumnList CreateColumns() {
      var result = new BindingColumnList {
        new BindingColumn(nameof(Set.SetNo)),
        new BindingColumn(nameof(Set.Act),
          typeof(ActList), nameof(Act.Name)),
        new BindingColumn(nameof(Set.Genre),
          typeof(GenreList), nameof(Genre.Name)),
        new BindingColumn(nameof(Set.Notes))
      };
      if (IsParentList) {
        result.Insert(0,
          new BindingColumn(nameof(Event.Date)));
        result.Insert(1,
          new BindingColumn(nameof(Event.Location)));
      }
      return result;
    }
  }
}