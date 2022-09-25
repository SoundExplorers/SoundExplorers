using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View; 

public class SelectRowMenuItem : ToolStripMenuItem {
  private Size _size;
  private string? _text;

  public SelectRowMenuItem() {
    ImageTransparentColor = Color.Black;
    Name = "SelectRowMenuItem";
    // The keyboard shortcut is built into DataGridView,
    // so here it is a dummy, represented by this text display.
    ShortcutKeyDisplayString = "Shift+Space";
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
    get => _text ?? (base.Text = _text = "&Select Row");
    set => base.Text = value;
  }
}