using System;
using System.Linq;
using System.Windows.Forms;

namespace SoundExplorers {
  /// <summary>
  ///   Contains the main entry point for the application.
  /// </summary>
  internal static class Program {
    /// <summary>
    ///   The main entry point for the application.
    /// </summary>
    /// <param name="args">
    ///   Command line arguments.
    /// </param>
    [STAThread]
    private static void Main(string[] args) {
      string entityTypeName = null;
      if (args.Length > 0) {
        entityTypeName = args[0];
      }
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      SplashManager.Show(typeof(SplashForm));
      var mdiParentForm = new MdiParentForm();
      Application.Run(mdiParentForm);
    }
  } //End of class
} //End of namespace