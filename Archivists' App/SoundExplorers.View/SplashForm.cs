using System.Windows.Forms;

namespace SoundExplorers.View {
  public class SplashForm : SplashFormBase {
    private PictureBox pictureBox1;
    private Label productNameLabel;

    public SplashForm() {
      // Required for Windows Form Designer support
      InitializeComponent();
      // Add any constructor code after InitializeComponent call
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///   Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources =
        new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
      this.productNameLabel = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.SplashPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1))
        .BeginInit();
      this.SuspendLayout();
      // 
      // SplashPanel
      // 
      this.SplashPanel.BackColor = System.Drawing.Color.Firebrick;
      this.SplashPanel.Controls.Add(this.productNameLabel);
      this.SplashPanel.Controls.Add(this.pictureBox1);
      this.SplashPanel.Controls.SetChildIndex(this.pictureBox1, 0);
      this.SplashPanel.Controls.SetChildIndex(this.productNameLabel, 0);
      this.SplashPanel.Controls.SetChildIndex(this.StatusLabel, 0);
      // 
      // productNameLabel
      // 
      this.productNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
      this.productNameLabel.Font = new System.Drawing.Font(
        "Old English Text MT", 27.75F, System.Drawing.FontStyle.Regular,
        System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.productNameLabel.ForeColor = System.Drawing.Color.Gold;
      this.productNameLabel.Location = new System.Drawing.Point(0, 0);
      this.productNameLabel.Name = "productNameLabel";
      this.productNameLabel.Size = new System.Drawing.Size(484, 71);
      this.productNameLabel.TabIndex = 4;
      this.productNameLabel.Text = "Sound Explorers Archive";
      this.productNameLabel.TextAlign =
        System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image =
        global::SoundExplorers.View.Properties.Resources.FredFlintstone;
      this.pictureBox1.ImageLocation = "";
      this.pictureBox1.Location = new System.Drawing.Point(142, 79);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(200, 200);
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // SplashForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(9, 20);
      this.BackColor = System.Drawing.Color.Gold;
      this.ClientSize = new System.Drawing.Size(500, 375);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "SplashForm";
      this.Text = "Batch Scheduler";
      this.SplashPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
    }

    #endregion
  } //End of class
} //End of namespace