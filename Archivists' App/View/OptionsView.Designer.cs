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
      this.BrowseButton = new System.Windows.Forms.Button();
      this.OkButton = new System.Windows.Forms.Button();
      this.CancelButton1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // DatabaseFolderLabel
      // 
      this.DatabaseFolderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DatabaseFolderLabel.Location = new System.Drawing.Point(11, 23);
      this.DatabaseFolderLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.DatabaseFolderLabel.Name = "DatabaseFolderLabel";
      this.DatabaseFolderLabel.Size = new System.Drawing.Size(151, 26);
      this.DatabaseFolderLabel.TabIndex = 0;
      this.DatabaseFolderLabel.Text = "&Database Folder";
      // 
      // DatabaseFolderTextBox
      // 
      this.DatabaseFolderTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DatabaseFolderTextBox.Location = new System.Drawing.Point(125, 23);
      this.DatabaseFolderTextBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.DatabaseFolderTextBox.Name = "DatabaseFolderTextBox";
      this.DatabaseFolderTextBox.Size = new System.Drawing.Size(613, 28);
      this.DatabaseFolderTextBox.TabIndex = 1;
      // 
      // BrowseButton
      // 
      this.BrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.BrowseButton.Location = new System.Drawing.Point(752, 12);
      this.BrowseButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.BrowseButton.Name = "BrowseButton";
      this.BrowseButton.Size = new System.Drawing.Size(119, 50);
      this.BrowseButton.TabIndex = 2;
      this.BrowseButton.Text = "&Browse...";
      this.BrowseButton.UseVisualStyleBackColor = true;
      this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
      // 
      // OkButton
      // 
      this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OkButton.Location = new System.Drawing.Point(619, 91);
      this.OkButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(119, 50);
      this.OkButton.TabIndex = 3;
      this.OkButton.Text = "&OK";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // CancelButton1
      // 
      this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CancelButton1.Location = new System.Drawing.Point(752, 91);
      this.CancelButton1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
      this.CancelButton1.Name = "CancelButton1";
      this.CancelButton1.Size = new System.Drawing.Size(119, 50);
      this.CancelButton1.TabIndex = 4;
      this.CancelButton1.Text = "Cancel";
      this.CancelButton1.UseVisualStyleBackColor = true;
      // 
      // OptionsView
      // 
      this.AcceptButton = this.OkButton;
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.CancelButton1;
      this.ClientSize = new System.Drawing.Size(882, 153);
      this.Controls.Add(this.CancelButton1);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.BrowseButton);
      this.Controls.Add(this.DatabaseFolderTextBox);
      this.Controls.Add(this.DatabaseFolderLabel);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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

    private System.Windows.Forms.Button BrowseButton;
    private System.Windows.Forms.Label DatabaseFolderLabel;
    private System.Windows.Forms.TextBox DatabaseFolderTextBox;

    #endregion

    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.Button CancelButton1;
  }
}