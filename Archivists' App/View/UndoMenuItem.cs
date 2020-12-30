using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  public class UndoMenuItem : ToolStripMenuItem {
    private Size _size;
    private string? _text;

    public UndoMenuItem() {
      ImageTransparentColor = Color.Black;
      Name = "UndoMenuItem";
      ShortcutKeys = Keys.Control | Keys.Z;
    }

    public override Size Size {
      get => _size != Size.Empty ? _size : base.Size = _size = new Size(198, 24);
      set => base.Size = value;
    }

    public override string Text {
      // Getter loops if compared with base instead of field.
      get => _text ?? (base.Text = _text = "&Undo");
      set => base.Text = value;
    }
  }
}