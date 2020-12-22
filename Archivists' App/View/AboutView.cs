using System;
using System.Reflection;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   About <see cref="Form" />.
  /// </summary>
  public partial class AboutView : Form {
    private static readonly Assembly EntryAssembly =
      Assembly.GetEntryAssembly();

    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="AboutView" /> class.
    /// </summary>
    public AboutView() {
      // Required for Windows Form Designer support.
      InitializeComponent();
      // Add any constructor code after InitializeComponent call.
      Load += OnLoad;
    }

    /// <summary>
    ///   Gets the copyright specified in the Copyright assembly property
    ///   in the top-level project file.
    /// </summary>
    private static string AssemblyCopyright {
      get {
        var assemblyCopyrightAttribute =
          (AssemblyCopyrightAttribute)EntryAssembly.GetCustomAttributes(
            typeof(AssemblyCopyrightAttribute), false)[0];
        return assemblyCopyrightAttribute.Copyright;
      }
    }

    private void OnLoad(object sender, EventArgs e) {
      ProductNameLabel.Text = Application.ProductName;
      // In .Net 5, Application.ProductVersion comes from the
      // InformationalVersion assembly property in the top-level project file,
      // not the ProductVersion property.
      VersionLabel.Text = "Version " + Application.ProductVersion; 
      CopyrightLabel.Text = AssemblyCopyright;
    }
  } //End of class
} //End of namespace