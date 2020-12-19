#if DEBUG
#else // Release build
using System.Threading;
#endif
using System;
using System.Windows.Forms;
using SoundExplorers.View;

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
      // Global exception handling code for release build only.
#if DEBUG
#else // Release build
      AddGlobalExceptionHandlers();
#endif
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      SplashManager.Show(typeof(SplashForm));
      Application.Run(MainView.Create());
    }

#if DEBUG
#else // Release build
    private static void AddGlobalExceptionHandlers() {
      // Add handler to handle the exception raised by main threads
      Application.ThreadException += Application_ThreadException;
      // Add handler to handle the exception raised by additional threads
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    /// <summary>
    ///   All exceptions thrown by the main thread are handled over this method.
    /// </summary>
    private static void Application_ThreadException
      (object sender, ThreadExceptionEventArgs e) {
      ShowExceptionDetails(e.Exception);
    }

    /// <summary>
    ///   All exceptions thrown by additional threads are handled in this method.
    /// </summary>
    private static void CurrentDomain_UnhandledException
      (object sender, UnhandledExceptionEventArgs e) {
      ShowExceptionDetails(e.ExceptionObject as Exception);
    }

    /// <summary>
    ///   Logs exception details and terminates the application.
    /// </summary>
    private static void ShowExceptionDetails(Exception ex) {
      MessageBox.Show(ex.ToString(),
        $"{Application.ProductName} - Terminal Error - "
        + "Press Ctrl+C to copy the diagnostics to the clipboard",
        MessageBoxButtons.OK, MessageBoxIcon.Error);
      Environment.Exit(0);
    }
#endif
  } //End of class
} //End of namespace