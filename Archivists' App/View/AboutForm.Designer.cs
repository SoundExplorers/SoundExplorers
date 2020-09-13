namespace SoundExplorers.View {
  partial class AboutForm {
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
      this.OKButtion = new System.Windows.Forms.Button();
      this.CopyrightLabel = new System.Windows.Forms.Label();
      this.ProductNameLabel = new System.Windows.Forms.Label();
      this.VersionLabel = new System.Windows.Forms.Label();
      this.PictureBox1 = new System.Windows.Forms.PictureBox();
      this.Panel1 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
      this.Panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // OKButtion
      // 
      this.OKButtion.BackColor = System.Drawing.Color.Firebrick;
      this.OKButtion.Cursor = System.Windows.Forms.Cursors.Default;
      this.OKButtion.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OKButtion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OKButtion.ForeColor = System.Drawing.Color.Gold;
      this.OKButtion.Location = new System.Drawing.Point(548, 238);
      this.OKButtion.Name = "OKButtion";
      this.OKButtion.Size = new System.Drawing.Size(102, 42);
      this.OKButtion.TabIndex = 5;
      this.OKButtion.Text = "OK";
      this.OKButtion.UseVisualStyleBackColor = false;
      // 
      // CopyrightLabel
      // 
      this.CopyrightLabel.AutoSize = true;
      this.CopyrightLabel.BackColor = System.Drawing.Color.Firebrick;
      this.CopyrightLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.CopyrightLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CopyrightLabel.ForeColor = System.Drawing.Color.Gold;
      this.CopyrightLabel.Location = new System.Drawing.Point(262, 158);
      this.CopyrightLabel.Name = "CopyrightLabel";
      this.CopyrightLabel.Size = new System.Drawing.Size(93, 20);
      this.CopyrightLabel.TabIndex = 7;
      this.CopyrightLabel.Text = "Copyright";
      this.CopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // ProductNameLabel
      // 
      this.ProductNameLabel.AutoSize = true;
      this.ProductNameLabel.BackColor = System.Drawing.Color.Firebrick;
      this.ProductNameLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.ProductNameLabel.Font = new System.Drawing.Font("Old English Text MT", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ProductNameLabel.ForeColor = System.Drawing.Color.Gold;
      this.ProductNameLabel.Location = new System.Drawing.Point(262, 30);
      this.ProductNameLabel.Name = "ProductNameLabel";
      this.ProductNameLabel.Size = new System.Drawing.Size(223, 40);
      this.ProductNameLabel.TabIndex = 8;
      this.ProductNameLabel.Text = "Product Name";
      this.ProductNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // VersionLabel
      // 
      this.VersionLabel.AutoSize = true;
      this.VersionLabel.BackColor = System.Drawing.Color.Firebrick;
      this.VersionLabel.Cursor = System.Windows.Forms.Cursors.Default;
      this.VersionLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.VersionLabel.ForeColor = System.Drawing.Color.Gold;
      this.VersionLabel.Location = new System.Drawing.Point(262, 100);
      this.VersionLabel.Name = "VersionLabel";
      this.VersionLabel.Size = new System.Drawing.Size(72, 20);
      this.VersionLabel.TabIndex = 9;
      this.VersionLabel.Text = "Version";
      this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // PictureBox1
      // 
      this.PictureBox1.BackColor = System.Drawing.SystemColors.Control;
      this.PictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
      this.PictureBox1.ForeColor = System.Drawing.SystemColors.ControlText;
      this.PictureBox1.Image = global::SoundExplorers.View.Properties.Resources.FredFlintstone;
      this.PictureBox1.Location = new System.Drawing.Point(29, 30);
      this.PictureBox1.Name = "PictureBox1";
      this.PictureBox1.Size = new System.Drawing.Size(200, 200);
      this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.PictureBox1.TabIndex = 6;
      this.PictureBox1.TabStop = false;
      // 
      // Panel1
      // 
      this.Panel1.BackColor = System.Drawing.Color.Firebrick;
      this.Panel1.Controls.Add(this.PictureBox1);
      this.Panel1.Controls.Add(this.OKButtion);
      this.Panel1.Controls.Add(this.VersionLabel);
      this.Panel1.Controls.Add(this.CopyrightLabel);
      this.Panel1.Controls.Add(this.ProductNameLabel);
      this.Panel1.Location = new System.Drawing.Point(16, 18);
      this.Panel1.Name = "Panel1";
      this.Panel1.Size = new System.Drawing.Size(732, 304);
      this.Panel1.TabIndex = 10;
      // 
      // AboutForm
      // 
      this.AcceptButton = this.OKButtion;
      this.AutoScaleBaseSize = new System.Drawing.Size(11, 25);
      this.BackColor = System.Drawing.Color.Gold;
      this.CancelButton = this.OKButtion;
      this.ClientSize = new System.Drawing.Size(767, 343);
      this.Controls.Add(this.Panel1);
      this.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "About MyApp";
      ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
      this.Panel1.ResumeLayout(false);
      this.Panel1.PerformLayout();
      this.ResumeLayout(false);
    }

    public System.Windows.Forms.Label CopyrightLabel;
    public System.Windows.Forms.Button OKButtion;
    public System.Windows.Forms.Panel Panel1;
    public System.Windows.Forms.PictureBox PictureBox1;
    public System.Windows.Forms.Label ProductNameLabel;
    public System.Windows.Forms.Label VersionLabel;

    #endregion
  }
}