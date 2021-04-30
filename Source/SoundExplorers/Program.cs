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
      ShowExceptionDetails((Exception)e.ExceptionObject);
    }

    /// <summary>
    ///   Logs exception details and terminates the application.
    /// </summary>
    private static void ShowExceptionDetails(Exception exception) {
      MessageWindow.Show(exception.ToString(),
        $"{Application.ProductName} - Terminal Error");
      Environment.Exit(0);
    }
#endif
  } //End of class
} //End of namespace