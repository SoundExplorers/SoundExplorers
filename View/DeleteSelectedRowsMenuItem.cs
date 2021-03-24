using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  public class DeleteSelectedRowsMenuItem : ToolStripMenuItem {
    private Size _size;
    private string? _text;

    public DeleteSelectedRowsMenuItem() {
      ImageTransparentColor = Color.Black;
      Name = "DeleteSelectedRowsMenuItem";
      ShortcutKeys = Keys.Control | Keys.Delete;
    }

    public override Size Size {
      get => _size != Size.Empty
        ? _size
        : base.Size = _size = new Size(EditContextMenuBase.ItemWidth,
          EditContextMenuBase.ItemHeight);
      set => base.Size = value;
    }

    public override string Text {
      // Getter loops if compared with base instead of field.
      get => _text ?? (base.Text = _text = "Delete Selected &Row(s)");
      set => base.Text = value;
    }
  }
}