using System.Drawing;
using System.Windows.Forms;

namespace SoundExplorers.View {
  public class DeleteMenuItem : ToolStripMenuItem {
    private Size _size;
    private string? _text;

    public DeleteMenuItem() {
      ImageTransparentColor = Color.Black;
      Name = "DeleteMenuItem";
      ShortcutKeys = Keys.Delete;
    }

    public override Size Size {
      get => _size != Size.Empty ? _size : base.Size = _size = new Size(198, 24);
      set => base.Size = value;
    }

    public override string Text {
      // Getter loops if compared with base instead of field.
      get => _text ?? (base.Text = _text = "&Delete");
      set => base.Text = value;
    }
  }
}