namespace SoundExplorers.View {
  partial class SplashForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

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
    ///   Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
      this.ProductNameTextBox = new System.Windows.Forms.TextBox();
      this.SplashPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // SplashPanel
      // 
      this.SplashPanel.BackColor = System.Drawing.Color.Firebrick;
      this.SplashPanel.Controls.Add(this.ProductNameTextBox);
      this.SplashPanel.Size = new System.Drawing.Size(484, 254);
      this.SplashPanel.Controls.SetChildIndex(this.StatusLabel, 0);
      this.SplashPanel.Controls.SetChildIndex(this.ProductNameTextBox, 0);
      // 
      // StatusLabel
      // 
      this.StatusLabel.Location = new System.Drawing.Point(0, 192);
      this.StatusLabel.Size = new System.Drawing.Size(484, 62);
      this.StatusLabel.Text = "Status Label";
      // 
      // ProductNameTextBox
      // 
      this.ProductNameTextBox.BackColor = System.Drawing.Color.Firebrick;
      this.ProductNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.ProductNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.ProductNameTextBox.Font = new System.Drawing.Font("Old English Text MT", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.ProductNameTextBox.ForeColor = System.Drawing.Color.Gold;
      this.ProductNameTextBox.Location = new System.Drawing.Point(0, 0);
      this.ProductNameTextBox.Multiline = true;
      this.ProductNameTextBox.Name = "ProductNameTextBox";
      this.ProductNameTextBox.ReadOnly = true;
      this.ProductNameTextBox.Size = new System.Drawing.Size(484, 109);
      this.ProductNameTextBox.TabIndex = 5;
      this.ProductNameTextBox.TabStop = false;
      this.ProductNameTextBox.Text = "Sound Explorers Audio Archive";
      this.ProductNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // SplashForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(11, 25);
      this.BackColor = System.Drawing.Color.Gold;
      this.ClientSize = new System.Drawing.Size(500, 270);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "SplashForm";
      this.Text = " ";
      this.SplashPanel.ResumeLayout(false);
      this.SplashPanel.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.TextBox ProductNameTextBox;
  }
}