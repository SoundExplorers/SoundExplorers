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
      this.productNameLabel = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.SplashPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // SplashPanel
      // 
      this.SplashPanel.BackColor = System.Drawing.Color.Firebrick;
      this.SplashPanel.Controls.Add(this.productNameLabel);
      this.SplashPanel.Controls.Add(this.pictureBox1);
      this.SplashPanel.Size = new System.Drawing.Size(566, 360);
      this.SplashPanel.Controls.SetChildIndex(this.pictureBox1, 0);
      this.SplashPanel.Controls.SetChildIndex(this.productNameLabel, 0);
      this.SplashPanel.Controls.SetChildIndex(this.StatusLabel, 0);
      // 
      // StatusLabel
      // 
      this.StatusLabel.Location = new System.Drawing.Point(0, 298);
      this.StatusLabel.Size = new System.Drawing.Size(566, 62);
      this.StatusLabel.Text = "Status Label";
      // 
      // productNameLabel
      // 
      this.productNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.productNameLabel.Font = new System.Drawing.Font("Old English Text MT", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.productNameLabel.ForeColor = System.Drawing.Color.Gold;
      this.productNameLabel.Location = new System.Drawing.Point(0, 0);
      this.productNameLabel.Name = "productNameLabel";
      this.productNameLabel.Size = new System.Drawing.Size(566, 89);
      this.productNameLabel.TabIndex = 4;
      this.productNameLabel.Text = "Sound Explorers Audio Archive";
      this.productNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.ImageLocation = "";
      this.pictureBox1.Location = new System.Drawing.Point(185, 92);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(200, 200);
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // SplashForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(11, 25);
      this.BackColor = System.Drawing.Color.Gold;
      this.ClientSize = new System.Drawing.Size(582, 376);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "SplashForm";
      this.Text = "Sound Explorers Audio Archive";
      this.SplashPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }
    
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label productNameLabel;

    #endregion
  }
}