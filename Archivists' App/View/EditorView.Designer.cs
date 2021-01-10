using System.Windows.Forms;

namespace SoundExplorers.View {
    partial class EditorView {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorView));
      this.ImageSplitContainer = new System.Windows.Forms.SplitContainer();
      this.GridSplitContainer = new System.Windows.Forms.SplitContainer();
      this.ParentGrid = new SoundExplorers.View.ParentGrid();
      this.MainGrid = new SoundExplorers.View.MainGrid();
      this.MissingImageLabel = new System.Windows.Forms.Label();
      this.FittedPictureBox1 = new SoundExplorers.View.FittedPictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.ImageSplitContainer)).BeginInit();
      this.ImageSplitContainer.Panel1.SuspendLayout();
      this.ImageSplitContainer.Panel2.SuspendLayout();
      this.ImageSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.GridSplitContainer)).BeginInit();
      this.GridSplitContainer.Panel1.SuspendLayout();
      this.GridSplitContainer.Panel2.SuspendLayout();
      this.GridSplitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ParentGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.MainGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.FittedPictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // ImageSplitContainer
      // 
      this.ImageSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ImageSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.ImageSplitContainer.Name = "ImageSplitContainer";
      this.ImageSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // ImageSplitContainer.Panel1
      // 
      this.ImageSplitContainer.Panel1.Controls.Add(this.GridSplitContainer);
      // 
      // ImageSplitContainer.Panel2
      // 
      this.ImageSplitContainer.Panel2.Controls.Add(this.MissingImageLabel);
      this.ImageSplitContainer.Panel2.Controls.Add(this.FittedPictureBox1);
      this.ImageSplitContainer.Panel2Collapsed = true;
      this.ImageSplitContainer.Size = new System.Drawing.Size(379, 322);
      this.ImageSplitContainer.SplitterDistance = 222;
      this.ImageSplitContainer.TabIndex = 4;
      // 
      // GridSplitContainer
      // 
      this.GridSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.GridSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.GridSplitContainer.Name = "GridSplitContainer";
      this.GridSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // GridSplitContainer.Panel1
      // 
      this.GridSplitContainer.Panel1.Controls.Add(this.ParentGrid);
      // 
      // GridSplitContainer.Panel2
      // 
      this.GridSplitContainer.Panel2.Controls.Add(this.MainGrid);
      this.GridSplitContainer.Size = new System.Drawing.Size(379, 322);
      this.GridSplitContainer.SplitterDistance = 152;
      this.GridSplitContainer.TabIndex = 4;
      // 
      // ParentGrid
      // 
      this.ParentGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ParentGrid.Location = new System.Drawing.Point(0, 0);
      this.ParentGrid.Name = "ParentGrid";
      this.ParentGrid.Size = new System.Drawing.Size(379, 152);
      this.ParentGrid.TabIndex = 3;
      // 
      // MainGrid
      // 
      this.MainGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MainGrid.Location = new System.Drawing.Point(0, 0);
      this.MainGrid.Name = "MainGrid";
      this.MainGrid.Size = new System.Drawing.Size(379, 166);
      this.MainGrid.TabIndex = 4;
      // 
      // MissingImageLabel
      // 
      this.MissingImageLabel.AllowDrop = true;
      this.MissingImageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.MissingImageLabel.Location = new System.Drawing.Point(0, 0);
      this.MissingImageLabel.Name = "MissingImageLabel";
      this.MissingImageLabel.Size = new System.Drawing.Size(150, 46);
      this.MissingImageLabel.TabIndex = 1;
      this.MissingImageLabel.Text = "You may drag an image file here.";
      this.MissingImageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.MissingImageLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.FittedPictureBox1_DragDrop);
      this.MissingImageLabel.DragOver += new System.Windows.Forms.DragEventHandler(this.FittedPictureBox1_DragOver);
      // 
      // FittedPictureBox1
      // 
      this.FittedPictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.FittedPictureBox1.Location = new System.Drawing.Point(0, 0);
      this.FittedPictureBox1.Name = "FittedPictureBox1";
      this.FittedPictureBox1.Size = new System.Drawing.Size(150, 46);
      this.FittedPictureBox1.TabIndex = 0;
      this.FittedPictureBox1.TabStop = false;
      this.FittedPictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.FittedPictureBox1_DragDrop);
      this.FittedPictureBox1.DragOver += new System.Windows.Forms.DragEventHandler(this.FittedPictureBox1_DragOver);
      this.FittedPictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FittedPictureBox1_MouseDown);
      // 
      // EditorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(379, 322);
      this.Controls.Add(this.ImageSplitContainer);
      this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "EditorView";
      this.Text = "EditorView";
      this.ImageSplitContainer.Panel1.ResumeLayout(false);
      this.ImageSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ImageSplitContainer)).EndInit();
      this.ImageSplitContainer.ResumeLayout(false);
      this.GridSplitContainer.Panel1.ResumeLayout(false);
      this.GridSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.GridSplitContainer)).EndInit();
      this.GridSplitContainer.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ParentGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.MainGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.FittedPictureBox1)).EndInit();
      this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.SplitContainer ImageSplitContainer;
        private System.Windows.Forms.SplitContainer GridSplitContainer;
        internal SoundExplorers.View.ParentGrid ParentGrid;
        internal SoundExplorers.View.MainGrid MainGrid;
        private FittedPictureBox FittedPictureBox1;
        private System.Windows.Forms.Label MissingImageLabel;

    }
}