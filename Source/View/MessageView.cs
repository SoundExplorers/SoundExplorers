using System;
using System.Windows.Forms;

namespace SoundExplorers.View {
  internal partial class MessageView : TextViewBase {
    /// <summary>
    ///   Initialises a new instance of the <see cref="MessageView" /> class.
    /// </summary>
    /// <param name="message">
    ///   The message to be shown.
    /// </param>
    /// <param name="title">
    ///   The title to be shown in the dialogue box's title bar.
    /// </param>
    public MessageView(string message, string title)
      : base(message, title) {
      InitializeComponent();
      StatusLabel.Text = string.Empty;
    }

    /// <summary>
    ///   Handles the Copy <see cref="Button" />'s
    ///   <see cref="Control.Click" /> event
    ///   to copy text to the clipboard.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void CopyButton_Click(object? sender, EventArgs e) {
      if (RichTextBox.SelectionLength == 0) {
        Clipboard.SetDataObject(RichTextBox.Text, true);
        StatusLabel.Text = "Text copied to clipboard";
      } else {
        RichTextBox.Copy();
        StatusLabel.Text = "Selected text copied to clipboard";
      }
      if (CopyButton.Focused) {
        OkButton.Focus();
      }
    }

    private void OKButton_Click(object? sender, EventArgs e) {
      Close();
    }
  }
}