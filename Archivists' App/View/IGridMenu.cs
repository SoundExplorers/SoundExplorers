using JetBrains.Annotations;

namespace SoundExplorers.View {
  internal interface IGridMenu {
    [NotNull] CutMenuItem CutMenuItem { get; }
    [NotNull] CopyMenuItem CopyMenuItem { get; }
    [NotNull] DeleteMenuItem DeleteMenuItem { get; }
    [NotNull] PasteMenuItem PasteMenuItem { get; }
    [NotNull] SelectAllMenuItem SelectAllMenuItem { get; }
    [NotNull] DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }
  }
}