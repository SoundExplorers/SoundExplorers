using System;
using System.Reflection;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   About <see cref="Form" />.
  /// </summary>
  public partial class AboutForm : Form {
    private static readonly Assembly EntryAssembly =
      Assembly.GetEntryAssembly();

    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="AboutForm" /> class.
    /// </summary>
    public AboutForm() {
      // Required for Windows Form Designer support.
      InitializeComponent();
      // Add any constructor code after InitializeComponent call.
      CopyrightLabel.Text = AssemblyCopyright;
      VersionLabel.Text = "Version " + Application.ProductVersion;
      ProductNameLabel.Text = Application.ProductName;
      Load += OnLoad;
    }

    private void OnLoad(object sender, EventArgs e) {
      Text = "About " + Application.ProductName;
    }

    /// <summary>
    ///   Gets the company specified for the assembly
    ///   in AssemblyInfo.cs.
    /// </summary>
    public static string AssemblyCompany {
      get {
        var assemblyCompanyAttribute =
          (AssemblyCompanyAttribute)EntryAssembly.GetCustomAttributes(
            typeof(AssemblyCompanyAttribute), false)[0];
        return assemblyCompanyAttribute.Company;
      }
    }

    /// <summary>
    ///   Gets the copyright specified for the assembly
    ///   in AssemblyInfo.cs.
    /// </summary>
    public static string AssemblyCopyright {
      get {
        var assemblyCopyrightAttribute =
          (AssemblyCopyrightAttribute)EntryAssembly.GetCustomAttributes(
            typeof(AssemblyCopyrightAttribute), false)[0];
        return assemblyCopyrightAttribute.Copyright;
      }
    }
  } //End of class
} //End of namespace