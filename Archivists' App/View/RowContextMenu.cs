using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  public class RowContextMenu : ContextMenuStrip {
    public RowContextMenu() {
      CutMenuItem = new CutMenuItem();
      CopyMenuItem = new CopyMenuItem();
      PasteMenuItem = new PasteMenuItem();
      SelectAllMenuItem = new SelectAllMenuItem();
      DeleteSelectedRowsMenuItem = new DeleteSelectedRowsMenuItem();
      ShowImageMargin = false;
      Size = new Size(61, 4);
    }

    internal CopyMenuItem CopyMenuItem { get; }
    internal CutMenuItem CutMenuItem { get; }
    internal DeleteSelectedRowsMenuItem DeleteSelectedRowsMenuItem { get; }
    internal PasteMenuItem PasteMenuItem { get; }
    internal SelectAllMenuItem SelectAllMenuItem { get; }

    public override ToolStripItemCollection Items {
      get {
        if (base.Items.Count == 0) {
          base.Items.AddRange(new ToolStripItem[] {
            CutMenuItem,
            CopyMenuItem,
            PasteMenuItem,
            SelectAllMenuItem,
            DeleteSelectedRowsMenuItem
          });
        }
        return base.Items;
      }
    }
  }
}