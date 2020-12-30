// Written by Simon O'Rorke, February 2005.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundExplorers.View {
	/// <summary>
	///   Base class for splash forms.
	/// </summary>
	/// <remarks>
	///   This class can be used in conjunction
	///   with class <see cref="SplashManager" />,
	///   which should be used to show, close and
	///   update application load status information
	///   on the splash window.  This class implements the
	///   <see cref="IMessageUpdater" /> interface
	///   to allow <see cref="SplashManager" /> to
	///   show application load status information
	///   on the splash window's
	///   <see cref="StatusLabel" />.
	///   <para>
	///     To set the width of the border
	///     around the edge of the splash form,
	///     use the
	///     <see cref="System.Windows.Forms.ScrollableControl.DockPadding" />
	///     property.
	///     To set the colour of the border
	///     around the edge of the splash form,
	///     use the
	///     <see cref="System.Windows.Forms.Form.BackColor" />
	///     property.
	///     To set the colour of the area within the border
	///     around the edge of the splash form,
	///     use the
	///     <see cref="System.Windows.Forms.Control.BackColor" />
	///     property of <see cref="SplashPanel" />.
	///   </para>
	///   <para>
	///     By default, the splash window will be shown centred on the screen
	///     and will be shown in the Windows task bar.
	///   </para>
	///   <para>
	///     By default, the form's
	///     <see cref="System.Windows.Forms.Control.Cursor" />
	///     will be set to
	///     <see cref="System.Windows.Forms.Cursors.AppStarting">
	///       Cursors.AppStarting
	///     </see>
	///     ,
	///     whose default appearance is an arrow and an hourglass.
	///   </para>
	///   <para>
	///     By default, the form's
	///     <see cref="System.Windows.Forms.Control.Text" />
	///     property, whose content will be shown in
	///     the Windows task bar, will be set to
	///     <see cref="System.Windows.Forms.Application.ProductName">
	///       Application.ProductName
	///     </see>
	///     ,
	///     as specified in the AssemblyProduct attribute
	///     in the entry assembly's AssemblyInfo.cs/AssemblyInfo.vb.
	///   </para>
	///   <para>
	///     <b>SplashFormBase</b> is not declared as
	///     abstract because an instance of
	///     <b>SplashFormBase</b> must be created
	///     when a derived form is shown in the designer.
	///   </para>
	/// </remarks>
	public class SplashFormBase : Form, IMessageUpdater {
    private string? _status = "";

    /// <summary>
    ///   Required designer variable.
    /// </summary>
    private IContainer? components;

    /// <summary>
    ///   A <see cref="System.Windows.Forms.Panel" />
    ///   onto which
    ///   <see cref="System.Windows.Forms.Control" />s
    ///   for the splash form can be placed.
    /// </summary>
    public Panel SplashPanel = null!;

    /// <summary>
    ///   A <see cref="System.Windows.Forms.Label" />
    ///   where information about the application
    ///   load status can be shown.
    /// </summary>
    public Label StatusLabel = null!;

    /// <summary>
    ///   Creates a new instance of the <see cref="SplashFormBase" /> class.
    /// </summary>
    protected SplashFormBase() {
      // Required for Windows Form Designer support
      InitializeComponent();
      // Add any constructor code after InitializeComponent call
      // If the main thread attempts to show a message box
      // over a splash window in a separate thread without
      // cross threading, as happens in the current version of
      // Anz.NZInfo.Updater.SelfUpdatingAppLoader.Load,
      // ensure that an InvalidOperationException
      // with message
      // "Cross-thread operation not valid: 
      //  Control 'SplashForm' accessed from a thread other 
      //  than the thread it was created on."
      // will not get thrown when the invoking application
      // is run from within the Visual Studio IDE.
      CheckForIllegalCrossThreadCalls = false;
      Load += OnLoad;
    }

    /// <summary>
    ///   Sets the application load status information
    ///   to be shown on the splash window.
    /// </summary>
    /// <param name="status">
    ///   The application load status information
    ///   to be shown on the splash window.
    /// </param>
    public void SetMessage(string status) {
      _status = status;
      ChangeStatusText();
    }

    private void OnLoad(object sender, EventArgs e) {
      Text = Application.ProductName;
    }

    private void ChangeStatusText() {
      if (InvokeRequired) {
        Invoke(new MethodInvoker(ChangeStatusText));
        return;
      }
      StatusLabel.Text = _status;
    }

    /// <summary>
    ///   Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        components?.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///   Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.SplashPanel = new System.Windows.Forms.Panel();
      this.StatusLabel = new System.Windows.Forms.Label();
      this.SplashPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // SplashPanel
      // 
      this.SplashPanel.BackColor = System.Drawing.Color.White;
      this.SplashPanel.Controls.Add(this.StatusLabel);
      this.SplashPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.SplashPanel.Location = new System.Drawing.Point(8, 8);
      this.SplashPanel.Name = "SplashPanel";
      this.SplashPanel.Size = new System.Drawing.Size(484, 359);
      this.SplashPanel.TabIndex = 2;
      // 
      // StatusLabel
      // 
      this.StatusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.StatusLabel.Location = new System.Drawing.Point(0, 327);
      this.StatusLabel.Name = "StatusLabel";
      this.StatusLabel.Size = new System.Drawing.Size(484, 32);
      this.StatusLabel.TabIndex = 2;
      this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // SplashFormBase
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(9, 20);
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(500, 375);
      this.Controls.Add(this.SplashPanel);
      this.Cursor = System.Windows.Forms.Cursors.AppStarting;
      this.DockPadding.All = 8;
      this.Font = new System.Drawing.Font("Verdana", 12F,
        System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
        ((System.Byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "SplashFormBase";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SplashFormBase";
      this.SplashPanel.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    #endregion
  } //End of class
} //End of namespace