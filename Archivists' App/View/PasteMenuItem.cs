using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  public class PasteMenuItem : ToolStripMenuItem {
    private Size _size;
    private string _text;

    public PasteMenuItem() {
      ImageTransparentColor = Color.Black;
      Name = "PasteMenuItem";
      ShortcutKeys = Keys.Control | Keys.V;
    }

    public override Size Size {
      get => _size != Size.Empty ? _size : base.Size = _size = new Size(198, 24);
      set => base.Size = value;
    }

    public override string Text {
      // Getter loops if compared with base instead of field.
      get => _text ?? (base.Text = _text = "&Paste");
      set => base.Text = value;
    }
  }
}