using System;
using System.Windows.Forms;
using SoundExplorers.Controller;

namespace SoundExplorers {
  /// <summary>
  ///   Contains the main entry point for the application.
  /// </summary>
  internal static class Program {
    /// <summary>
    ///   The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      SplashManager.Show(typeof(SplashForm));
      Application.Run((MainView)ViewFactory.Create<MainView, MainController>());
    }
  } //End of class
} //End of namespace