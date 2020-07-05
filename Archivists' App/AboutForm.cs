using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace SoundExplorers {

    /// <summary>
    /// About <see cref="Form"/>.
    /// </summary>
    internal class AboutForm : System.Windows.Forms.Form {

        #region Generated Code
        #region Form Controls
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.Button OKButtion;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.Label ProductNameLabel;
        private System.Windows.Forms.Label VersionLabel;
        private Panel Panel1;
        private System.ComponentModel.IContainer components = null;
        #endregion Form Controls

        #region Dispose
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion Dispose

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
            this.OKButtion.Location = new System.Drawing.Point(448, 190);
            this.OKButtion.Name = "OKButtion";
            this.OKButtion.Size = new System.Drawing.Size(84, 34);
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
            this.CopyrightLabel.Location = new System.Drawing.Point(248, 107);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(71, 16);
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
            this.ProductNameLabel.Location = new System.Drawing.Point(248, 24);
            this.ProductNameLabel.Name = "ProductNameLabel";
            this.ProductNameLabel.Size = new System.Drawing.Size(181, 32);
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
            this.VersionLabel.Location = new System.Drawing.Point(248, 70);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(56, 16);
            this.VersionLabel.TabIndex = 9;
            this.VersionLabel.Text = "Version";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PictureBox1
            // 
            this.PictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.PictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.PictureBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PictureBox1.Image = global::SoundExplorers.Properties.Resources.FredFlintstone;
            this.PictureBox1.Location = new System.Drawing.Point(24, 24);
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
            this.Panel1.Location = new System.Drawing.Point(13, 14);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(556, 248);
            this.Panel1.TabIndex = 10;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.OKButtion;
            this.AutoScaleBaseSize = new System.Drawing.Size(9, 20);
            this.BackColor = System.Drawing.Color.Gold;
            this.CancelButton = this.OKButtion;
            this.ClientSize = new System.Drawing.Size(584, 276);
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
        #endregion
        #endregion Generated Code
 
        #region Fields
        private static Assembly _entryAssembly = Assembly.GetEntryAssembly();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="AboutForm"/> class.
        /// </summary>
        public AboutForm() {
            // Required for Windows Form Designer support.
            InitializeComponent();
            // Add any constructor code after InitializeComponent call.
            this.Text = "About " + Application.ProductName;
            this.CopyrightLabel.Text = AssemblyCopyright;
            this.VersionLabel.Text = "Version " + Application.ProductVersion;
            this.ProductNameLabel.Text = Application.ProductName;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the company specified for the assembly
        /// in AssemblyInfo.cs.
        /// </summary>
        public static string AssemblyCompany {
            get {
                AssemblyCompanyAttribute assemblyCompanyAttribute = 
                    (AssemblyCompanyAttribute)(
                    _entryAssembly.GetCustomAttributes(
                    typeof(AssemblyCompanyAttribute), false)[0]);
                return assemblyCompanyAttribute.Company;
            }
        }

        /// <summary>
        /// Gets the copyright specified for the assembly
        /// in AssemblyInfo.cs.
        /// </summary>
        public static string AssemblyCopyright {
            get {
                AssemblyCopyrightAttribute assemblyCopyrightAttribute = 
                    (AssemblyCopyrightAttribute)(
                    _entryAssembly.GetCustomAttributes(
                    typeof(AssemblyCopyrightAttribute), false)[0]);
                return assemblyCopyrightAttribute.Copyright;
            }
        }
        #endregion Properties
    }//End of class
}//End of namespace
