using System.ComponentModel;

namespace SoundExplorers.View {
  partial class OptionsView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsView));
      this.DatabaseFolderLabel = new System.Windows.Forms.Label();
      this.DatabaseFolderTextBox = new System.Windows.Forms.TextBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.MessageTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // DatabaseFolderLabel
      // 
      this.DatabaseFolderLabel.AutoSize = true;
      this.DatabaseFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.DatabaseFolderLabel.Location = new System.Drawing.Point(14, 12);
      this.DatabaseFolderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.DatabaseFolderLabel.Name = "DatabaseFolderLabel";
      this.DatabaseFolderLabel.Size = new System.Drawing.Size(121, 18);
      this.DatabaseFolderLabel.TabIndex = 1;
      this.DatabaseFolderLabel.Text = "&Database Folder:";
      // 
      // DatabaseFolderTextBox
      // 
      this.DatabaseFolderTextBox.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.DatabaseFolderTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.DatabaseFolderTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.DatabaseFolderTextBox.Location = new System.Drawing.Point(161, 12);
      this.DatabaseFolderTextBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.DatabaseFolderTextBox.Multiline = true;
      this.DatabaseFolderTextBox.Name = "DatabaseFolderTextBox";
      this.DatabaseFolderTextBox.ReadOnly = true;
      this.DatabaseFolderTextBox.Size = new System.Drawing.Size(601, 73);
      this.DatabaseFolderTextBox.TabIndex = 2;
      this.DatabaseFolderTextBox.Text = "D:\\Simon\\OneDrive\\Documents\\Software\\Sound Explorers Audio Archive\\Database D:\\Si" +
    "mon\\OneDrive\\Documents\\Software\\Sound Explorers Audio Archive\\Database";
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.OkButton.Location = new System.Drawing.Point(673, 260);
      this.OkButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(100, 45);
      this.OkButton.TabIndex = 0;
      this.OkButton.Text = "&OK";
      this.OkButton.UseVisualStyleBackColor = true;
      // 
      // MessageTextBox
      // 
      this.MessageTextBox.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.MessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.MessageTextBox.Location = new System.Drawing.Point(161, 105);
      this.MessageTextBox.Multiline = true;
      this.MessageTextBox.Name = "MessageTextBox";
      this.MessageTextBox.ReadOnly = true;
      this.MessageTextBox.Size = new System.Drawing.Size(498, 200);
      this.MessageTextBox.TabIndex = 3;
      this.MessageTextBox.Text = resources.GetString("MessageTextBox.Text");
      // 
      // OptionsView
      // 
      this.AcceptButton = this.OkButton;
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.OkButton;
      this.ClientSize = new System.Drawing.Size(784, 317);
      this.Controls.Add(this.MessageTextBox);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.DatabaseFolderTextBox);
      this.Controls.Add(this.DatabaseFolderLabel);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "OptionsView";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Options";
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private System.Windows.Forms.Label DatabaseFolderLabel;
    private System.Windows.Forms.TextBox DatabaseFolderTextBox;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.TextBox MessageTextBox;
  }
}