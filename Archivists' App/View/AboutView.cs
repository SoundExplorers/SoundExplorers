using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SoundExplorers.View {
  /// <summary>
  ///   About <see cref="Form" />.
  /// </summary>
  public partial class AboutView : Form {
    private Assembly? _entryAssembly;

    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="AboutView" /> class.
    /// </summary>
    public AboutView() {
      // Required for Windows Form Designer support.
      InitializeComponent();
      // Add any constructor code after InitializeComponent call.
      ProductNameLabel.Text = Application.ProductName;
      // In .Net 5, Application.ProductVersion comes from the
      // InformationalVersion assembly property in the top-level project file,
      // not the ProductVersion property.
      VersionLabel.Text = $"Version {Application.ProductVersion}";
      CopyrightLabel.Text = AssemblyCopyright;
      LicenceButton.Click += LicenceButton_Click;
    }

    /// <summary>
    ///   Gets the copyright specified in the Copyright assembly property
    ///   in the top-level project file.
    /// </summary>
    private string AssemblyCopyright {
      get {
        var assemblyCopyrightAttribute =
          (AssemblyCopyrightAttribute)EntryAssembly.GetCustomAttributes(
            typeof(AssemblyCopyrightAttribute), false)[0];
        return assemblyCopyrightAttribute.Copyright;
      }
    }

    private Assembly EntryAssembly => _entryAssembly ??= Assembly.GetEntryAssembly()!;

    /// <summary>
    ///   Returns the text contained in the
    ///   the specified embedded resource file
    ///   in the entry (executable/top-level) assembly.
    /// </summary>
    /// <param name="filename">
    ///   The name of an embedded resource file
    ///   in the entry assembly.
    /// </param>
    /// <remarks>
    ///   To embed a file in an assembly,
    ///   add it to the assembly's project
    ///   and set the file's Build Action property to
    ///   "Embedded Resource".
    ///   To access an embedded resource file that is
    ///   in a folder within the project,
    ///   <paramref name="filename" /> must be prefixed
    ///   by the folder name followed a dot.
    ///   For example, to get the text in file "Test.sql"
    ///   in folder "Embedded" in the executing assembly's project:
    ///   <code>
    ///     // C#
    ///     string text = GetEmbeddedText("Embedded.Test.sql");
    ///     </code>
    /// </remarks>
    /// <exception cref="FileNotFoundException">
    ///   The specified embedded resource file
    ///   cannot be found in the executing assembly.
    /// </exception>
    private string GetEmbeddedText(string filename) {
      string assemblyName = EntryAssembly.GetName().Name!;
      var stream =
        EntryAssembly.GetManifestResourceStream($"{assemblyName}.{filename}")
        ??
        throw new FileNotFoundException(
          "Embedded resource file \"" +
          filename +
          "\" cannot be found in assembly " +
          assemblyName +
          ". (To be included in an assembly as " +
          "an embedded resource, a file's " +
          "Build Action property must be set to " +
          "\"Embedded Resource\" in the assembly's project.)",
          filename);
      using var reader = new StreamReader(stream);
      return reader.ReadToEnd();
    }

    private void LicenceButton_Click(object? sender, EventArgs e) {
      MessageWindow.Show(this,
        GetEmbeddedText("Licence.txt"),
        $"{Application.ProductName} - Licence");
    }
  } //End of class
} //End of namespace