using System.ComponentModel;

namespace SoundExplorers.View {
  partial class TextViewBase {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.Panel = new System.Windows.Forms.Panel();
      this.RichTextBox = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // Panel
      // 
      this.Panel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.Panel.Location = new System.Drawing.Point(0, -48);
      this.Panel.Name = "Panel";
      this.Panel.Size = new System.Drawing.Size(303, 100);
      this.Panel.TabIndex = 0;
      // 
      // RichTextBox
      // 
      this.RichTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.RichTextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.RichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.RichTextBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.RichTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
      this.RichTextBox.Location = new System.Drawing.Point(0, 0);
      this.RichTextBox.MaxLength = 0;
      this.RichTextBox.MinimumSize = new System.Drawing.Size(244, 22);
      this.RichTextBox.Name = "RichTextBox";
      this.RichTextBox.ReadOnly = true;
      this.RichTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.RichTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.RichTextBox.Size = new System.Drawing.Size(303, 22);
      this.RichTextBox.TabIndex = 3;
      this.RichTextBox.Text = "";
      // 
      // TextViewBase
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(9, 20);
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(303, 52);
      this.Controls.Add(this.RichTextBox);
      this.Controls.Add(this.Panel);
      this.Cursor = System.Windows.Forms.Cursors.Default;
      this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Location = new System.Drawing.Point(184, 250);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(321, 99);
      this.Name = "TextViewBase";
      this.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "TextViewBase";
      this.Load += new System.EventHandler(this.TextViewBase_Load);
      this.ResumeLayout(false);

    }

    protected System.Windows.Forms.Panel Panel;
    protected System.Windows.Forms.RichTextBox RichTextBox;
    #endregion
  }
}