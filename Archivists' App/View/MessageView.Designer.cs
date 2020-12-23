using System.ComponentModel;

namespace SoundExplorers.View {
  partial class MessageView {
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
      this.StatusLabel = new System.Windows.Forms.Label();
      this.OkButton = new System.Windows.Forms.Button();
      this.CopyButton = new System.Windows.Forms.Button();
      this.Panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // Panel
      // 
      this.Panel.Controls.Add(this.OkButton);
      this.Panel.Controls.Add(this.CopyButton);
      this.Panel.Controls.Add(this.StatusLabel);
      this.Panel.Location = new System.Drawing.Point(0, 23);
      this.Panel.Size = new System.Drawing.Size(310, 100);
      // 
      // RichTextBox
      // 
      this.RichTextBox.Size = new System.Drawing.Size(310, 23);
      // 
      // StatusLabel
      // 
      this.StatusLabel.BackColor = System.Drawing.SystemColors.Control;
      this.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.StatusLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.StatusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.StatusLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.StatusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
      this.StatusLabel.Location = new System.Drawing.Point(0, 66);
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.StatusLabel.Size = new System.Drawing.Size(310, 34);
      this.StatusLabel.TabIndex = 8;
      this.StatusLabel.Text = "lblStatus";
      this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // OkButton
      // 
      this.OkButton.BackColor = System.Drawing.SystemColors.Control;
      this.OkButton.Cursor = System.Windows.Forms.Cursors.Default;
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.OkButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.OkButton.ForeColor = System.Drawing.SystemColors.ControlText;
      this.OkButton.Location = new System.Drawing.Point(25, 16);
      this.OkButton.Name = "OkButton";
      this.OkButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.OkButton.Size = new System.Drawing.Size(65, 34);
      this.OkButton.TabIndex = 9;
      this.OkButton.Text = "O&K";
      this.OkButton.UseVisualStyleBackColor = false;
      this.OkButton.Click += new System.EventHandler(this.OKButton_Click);
      // 
      // CopyButton
      // 
      this.CopyButton.BackColor = System.Drawing.SystemColors.Control;
      this.CopyButton.Cursor = System.Windows.Forms.Cursors.Default;
      this.CopyButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.CopyButton.ForeColor = System.Drawing.SystemColors.ControlText;
      this.CopyButton.Location = new System.Drawing.Point(100, 16);
      this.CopyButton.Name = "CopyButton";
      this.CopyButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.CopyButton.Size = new System.Drawing.Size(173, 34);
      this.CopyButton.TabIndex = 10;
      this.CopyButton.Text = "&Copy to Clipboard";
      this.CopyButton.UseVisualStyleBackColor = false;
      this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
      // 
      // MessageView
      // 
      this.AcceptButton = this.OkButton;
      this.CancelButton = this.OkButton;
      this.ClientSize = new System.Drawing.Size(310, 123);
      this.Location = new System.Drawing.Point(0, 0);
      this.Name = "MessageView";
      this.Text = "MessageView";
      this.Panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }


    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.Button CopyButton;
    private System.Windows.Forms.Label StatusLabel;
    #endregion
  }
}