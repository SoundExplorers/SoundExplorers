namespace SoundExplorers.View {
  partial class AboutView {
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutView));
      this.OKButtion = new System.Windows.Forms.Button();
      this.CopyrightLabel = new System.Windows.Forms.Label();
      this.ProductNameLabel = new System.Windows.Forms.Label();
      this.VersionLabel = new System.Windows.Forms.Label();
      this.Panel1 = new System.Windows.Forms.Panel();
      this.LicenceButton = new System.Windows.Forms.Button();
      this.PictureBox1 = new System.Windows.Forms.PictureBox();
      this.Panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // OKButtion
      // 
      this.OKButtion.BackColor = System.Drawing.Color.Firebrick;
      this.OKButtion.Cursor = System.Windows.Forms.Cursors.Default;
      this.OKButtion.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OKButtion.FlatAppearance.BorderColor = System.Drawing.Color.Gold;
      this.OKButtion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.OKButtion.ForeColor = System.Drawing.Color.Gold;
      this.OKButtion.Location = new System.Drawing.Point(668, 265);
      this.OKButtion.Name = "OKButtion";
      this.OKButtion.Size = new System.Drawing.Size(101, 42);
      this.OKButtion.TabIndex = 5;
      this.OKButtion.Text = "OK";
      this.OKButtion.UseVisualStyleBackColor = false;
      // 
      // CopyrightLabel
      // 
      this.CopyrightLabel.AutoSize = true;
      this.CopyrightLabel.BackColor = System.Drawing.Color.Firebrick;
      this.CopyrightLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.CopyrightLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.CopyrightLabel.ForeColor = System.Drawing.Color.Gold;
      this.CopyrightLabel.Location = new System.Drawing.Point(258, 195);
      this.CopyrightLabel.Name = "CopyrightLabel";
      this.CopyrightLabel.Size = new System.Drawing.Size(516, 20);
      this.CopyrightLabel.TabIndex = 7;
      this.CopyrightLabel.Text = "Copyright © 2021-22 Sound and Light Exploration Society";
      this.CopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // ProductNameLabel
      // 
      this.ProductNameLabel.AutoSize = true;
      this.ProductNameLabel.BackColor = System.Drawing.Color.Firebrick;
      this.ProductNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.ProductNameLabel.Font = new System.Drawing.Font("Old English Text MT", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.ProductNameLabel.ForeColor = System.Drawing.Color.Gold;
      this.ProductNameLabel.Location = new System.Drawing.Point(250, 67);
      this.ProductNameLabel.Name = "ProductNameLabel";
      this.ProductNameLabel.Size = new System.Drawing.Size(460, 40);
      this.ProductNameLabel.TabIndex = 8;
      this.ProductNameLabel.Text = "Sound Explorers Audio Archive";
      this.ProductNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // VersionLabel
      // 
      this.VersionLabel.AutoSize = true;
      this.VersionLabel.BackColor = System.Drawing.Color.Firebrick;
      this.VersionLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.VersionLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.VersionLabel.ForeColor = System.Drawing.Color.Gold;
      this.VersionLabel.Location = new System.Drawing.Point(259, 138);
      this.VersionLabel.Name = "VersionLabel";
      this.VersionLabel.Size = new System.Drawing.Size(150, 20);
      this.VersionLabel.TabIndex = 9;
      this.VersionLabel.Text = "Version 2020.01";
      this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // Panel1
      // 
      this.Panel1.BackColor = System.Drawing.Color.Firebrick;
      this.Panel1.Controls.Add(this.LicenceButton);
      this.Panel1.Controls.Add(this.PictureBox1);
      this.Panel1.Controls.Add(this.OKButtion);
      this.Panel1.Controls.Add(this.VersionLabel);
      this.Panel1.Controls.Add(this.CopyrightLabel);
      this.Panel1.Controls.Add(this.ProductNameLabel);
      this.Panel1.Location = new System.Drawing.Point(12, 12);
      this.Panel1.Name = "Panel1";
      this.Panel1.Size = new System.Drawing.Size(778, 318);
      this.Panel1.TabIndex = 10;
      // 
      // LicenceButton
      // 
      this.LicenceButton.BackColor = System.Drawing.Color.Firebrick;
      this.LicenceButton.Cursor = System.Windows.Forms.Cursors.Default;
      this.LicenceButton.FlatAppearance.BorderColor = System.Drawing.Color.Gold;
      this.LicenceButton.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.LicenceButton.ForeColor = System.Drawing.Color.Gold;
      this.LicenceButton.Location = new System.Drawing.Point(530, 265);
      this.LicenceButton.Name = "LicenceButton";
      this.LicenceButton.Size = new System.Drawing.Size(126, 42);
      this.LicenceButton.TabIndex = 11;
      this.LicenceButton.Text = "&Licence...";
      this.LicenceButton.UseVisualStyleBackColor = false;
      // 
      // PictureBox1
      // 
      this.PictureBox1.Image = global::SoundExplorers.View.Properties.Resources.Kettle_Drum_256x256;
      this.PictureBox1.Location = new System.Drawing.Point(1, 33);
      this.PictureBox1.Name = "PictureBox1";
      this.PictureBox1.Size = new System.Drawing.Size(247, 258);
      this.PictureBox1.TabIndex = 10;
      this.PictureBox1.TabStop = false;
      // 
      // AboutView
      // 
      this.AcceptButton = this.OKButtion;
      this.AutoScaleBaseSize = new System.Drawing.Size(11, 25);
      this.BackColor = System.Drawing.Color.Gold;
      this.CancelButton = this.OKButtion;
      this.ClientSize = new System.Drawing.Size(800, 343);
      this.Controls.Add(this.Panel1);
      this.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutView";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "About MyApp";
      this.Panel1.ResumeLayout(false);
      this.Panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion
    
    public System.Windows.Forms.Label CopyrightLabel;
    public System.Windows.Forms.Button LicenceButton;
    public System.Windows.Forms.Button OKButtion;
    public System.Windows.Forms.Panel Panel1;
    private System.Windows.Forms.PictureBox PictureBox1;
    public System.Windows.Forms.Label ProductNameLabel;
    public System.Windows.Forms.Label VersionLabel;
  }
}