using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class PieceList : EntityListBase<Piece, PieceBindingItem> {
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public PieceList() : this(true) { }

    public PieceList(bool isChildList) :
      base(isChildList ? typeof(SetList) : null) { }

    protected override PieceBindingItem CreateBindingItem(Piece piece) {
      return new PieceBindingItem {
        Date = piece.Set.Event.Date, 
        Location = piece.Set.Event.Location.Name!,
        SetNo = piece.Set.SetNo,
        PieceNo = piece.PieceNo,
        Title = piece.Title,
        Duration = piece.Duration,
        AudioUrl = piece.AudioUrl,
        VideoUrl = piece.VideoUrl,
        Notes = piece.Notes
      };
    }

    protected override TypedBindingList<Piece, PieceBindingItem> CreateBindingList(
      IList<PieceBindingItem> list) {
      return new PieceBindingList(list);
    }

    protected override BindingColumnList CreateColumns() {
      var result = new BindingColumnList {
        new BindingColumn(nameof(Event.Date), typeof(DateTime)) {
          IsVisible = !IsChildList
        },
        new BindingColumn(nameof(Event.Location), typeof(string)) {
          IsVisible = !IsChildList
        },
        new BindingColumn(nameof(Set.SetNo), typeof(int)) {
          IsVisible = !IsChildList
        },
        new BindingColumn(nameof(Piece.PieceNo), typeof(int)) {IsInKey = true},
        new BindingColumn(nameof(Piece.Title), typeof(string)),
        new BindingColumn(nameof(Piece.Duration), typeof(TimeSpan)),
        new BindingColumn(nameof(Piece.AudioUrl), typeof(string)) {DisplayName = "Audio URL"},
        new BindingColumn(nameof(Piece.VideoUrl), typeof(string)) {DisplayName = "Video URL"},
        new BindingColumn(nameof(Piece.Notes), typeof(string))
      };
      return result;
    }

    public override IdentifyingParentChildren GetIdentifyingParentChildrenForMainList(
      int rowIndex) {
      return new IdentifyingParentChildren(this[rowIndex],
        this[rowIndex].Credits.Values.ToList());
    }
  }
}