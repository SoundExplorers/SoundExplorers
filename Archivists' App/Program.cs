using System;
#if DEBUG
#else // Release build
using System.Threading;
#endif
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
      // For the global exception handling code, see Rahul Modi's answer in
      // https://stackoverflow.com/questions/8148156/winforms-global-exception-handling
#if DEBUG
#else // Release build
      AddGlobalExceptionHandlers();
#endif
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

    private static void Application_ThreadException
      (object sender, ThreadExceptionEventArgs e) {
      // All exceptions thrown by the main thread are handled over this method
      ShowExceptionDetails(e.Exception);
    }

    private static void CurrentDomain_UnhandledException
      (object sender, UnhandledExceptionEventArgs e) {
      // All exceptions thrown by additional threads are handled in this method
      ShowExceptionDetails(e.ExceptionObject as Exception);
      // Suspend the current thread for now to stop the exception from throwing.
      // The suppressed compiler warning is that Thread.Suspend is obsolete,
      // but I don't see the alternative in this case:  see Rahul Modi's 
      // comments in the Stack Overflow article cited in Main.
#pragma warning disable 618
      Thread.CurrentThread.Suspend();
#pragma warning restore 618
    }

    private static void ShowExceptionDetails(Exception ex) {
      // Do logging of exception details
      MessageBox.Show(ex.ToString(), ex.TargetSite.ToString(),
        MessageBoxButtons.OK, MessageBoxIcon.Error);
      // Stop the application and any threads in suspended state.
      // See Rahul Modi's comments in the Stack Overflow article cited above.
      Environment.Exit(-1);
    }
#endif
  } //End of class
} //End of namespace