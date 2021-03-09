using System;
using System.Collections;
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

    internal SetList SetList { get; private set; } = null!;

    protected override PieceBindingItem CreateBindingItem(Piece piece) {
      return new PieceBindingItem {
        Date = piece.Set.Event.Date,
        Location = piece.Set.Event.Location.Name!,
        SetNo = piece.Set.SetNo.ToString(),
        PieceNo = piece.PieceNo.ToString(),
        Title = piece.Title,
        Duration = piece.Duration.ToString(piece.Duration < TimeSpan.FromHours(1)
          ? "m\\:ss"
          : "h\\:mm\\:ss"),
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
          IsVisible = ListRole != ListRole.Child
        },
        new BindingColumn(nameof(Event.Location), typeof(string)) {
          IsVisible = ListRole != ListRole.Child
        },
        new BindingColumn(nameof(Set.SetNo), typeof(int)) {
          IsVisible = ListRole != ListRole.Child
        },
        new BindingColumn(nameof(Piece.PieceNo), typeof(int)) {IsInKey = true},
        new BindingColumn(nameof(Piece.Title), typeof(string)),
        new BindingColumn(nameof(Piece.Duration), typeof(TimeSpan)),
        new BindingColumn(nameof(Piece.AudioUrl), typeof(string))
          {DisplayName = "Audio URL"},
        new BindingColumn(nameof(Piece.VideoUrl), typeof(string))
          {DisplayName = "Video URL"},
        new BindingColumn(nameof(Piece.Notes), typeof(string))
      };
      return result;
    }

    protected override IComparer<Piece> CreateEntityComparer() {
      return new PieceComparer();
    }

    public override IEntityList CreateParentList() {
      return SetList = new SetList(false) {Session = Session};
    }

    protected override IList GetChildList(int rowIndex) {
      return this[rowIndex].Credits.Values.ToList();
    }
  }
}