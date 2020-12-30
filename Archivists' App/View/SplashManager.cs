//-----------------------------------------------------------------------
// <copyright 
//     file="SplashForm.cs" 
//     company="ANZ National Bank Limited" 
//     author="Neil Becker and Simon O'Rorke">
//     Copyright (c) ANZ National Bank Limited.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
// Based on
//   How to do Application Initialization while showing a SplashScreen
//     http://www.thecodeproject.com/csharp/apploadingarticle.asp

using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SoundExplorers.View {
  /// <summary>
  ///   A class for managing the application splash screen.
  ///   The Splash screen is loaded in its own thread (i.e.,
  ///   has its own message pump). This ensures that the form
  ///   redraws correctly.
  /// </summary>
  /// <remarks>
  ///   You cannot create a new instance of the <b>SplashManager</b> class.
  ///   All members are static (<b>Shared</b> in Visual Basic).
  ///   To show a splash
  ///   <see cref="Form" />
  ///   window, call the
  ///   <see cref="O:Anz.NZInfo.SplashManager.Show">Show</see>
  ///   method.
  ///   To show information about the application load status
  ///   on the splash window, use the <see cref="Status" /> property.
  ///   To allow the <see cref="Status" /> property to be set, the splash
  ///   <see cref="Form" /> must implement the
  ///   <see cref="IMessageUpdater" /> interface.
  ///   To close the splash window, call the
  ///   <see cref="Close" /> method.
  ///   <para>
  ///     In order ensure that the main <see cref="Form" />
  ///     will be shown in foreground (focused)
  ///     when a splash window hs been shown with
  ///     <b>SplashManager</b>,
  ///     (i.e. even when
  ///     a message box has not previously been shown
  ///     in front of the splash form),
  ///     the main form's
  ///     <see cref="Control.VisibleChanged" />
  ///     event handler must invoke the main Form's
  ///     <see cref="Form.Activate" />
  ///     method and then invoke
  ///     <see cref="Close">SplashManager.Close</see>.
  ///   </para>
  /// </remarks>
  /// <example>
  ///   Loading a
  ///   <see cref="Form" />
  ///   called MainForm while showing details of the
  ///   load progress on a splash
  ///   <see cref="Form" />
  ///   called SplashForm that implements the
  ///   <see cref="IMessageUpdater" /> interface
  ///   can be achieved by including code similar to
  ///   the following in MainForm.
  ///   <code>
  ///  // C#
  ///  using System.Windows.Forms;
  ///  using Anz.NZInfo;
  ///  
  ///  [STAThread]
  ///  static void Main(string[] args) {
  /// 	    SplashManager.Show(typeof(SplashForm));
  /// 	    MainForm mainForm = new MainForm();
  /// 	    Application.Run(mainForm);
  ///  }
  /// 
  ///  public MainForm() {
  /// 	    // Required for Windows Form Designer support
  /// 	    InitializeComponent();
  /// 	    // Do some fake startup, showing progress
  /// 	    // on the splash window.
  /// 	    SplashManager.Status = "Loading Files...";
  /// 	    System.Threading.Thread.Sleep(2000);
  /// 	    SplashManager.Status = "Loading Plug/Ins...";
  /// 	    System.Threading.Thread.Sleep(2000);
  /// 	    SplashManager.Status = "Connecting to Database...";
  /// 	    System.Threading.Thread.Sleep(2000);
  ///  }
  /// 
  ///  private void MainForm_VisibleChanged(object? sender, System.EventArgs e) {
  /// 	    // Bring the form to the foreground when the load is complete.
  /// 	    // If you do not do this before closing the splash window,
  /// 	    // the main window will not show in foreground
  /// 	    // unless a message box has previously been shown
  ///      // in front of the splash form.
  /// 	    this.Activate();
  /// 	    SplashManager.Close();
  ///  }
  ///  </code>
  /// </example>
  public static class SplashManager {
    private static object[]? _args;
    private static bool _isSplashFormBeingShown;

    /// <summary>
    ///   The splash form.
    /// </summary>
    private static Form? _splashForm;

    private static Type _splashFormType = null!;

    /// <summary>
    ///   A background thread to run the splash form in.
    /// </summary>
    private static Thread? _splashThread;

    private static string? _status;

    /// <summary>
    ///   Gets the Splash <see cref="Form" />.
    /// </summary>
    /// <remarks>
    ///   If the background thread has not finished
    ///   showing the splash form, a null reference
    ///   (<b>Nothing</b> in Visual Basic) will be returned.
    /// </remarks>
    public static Form SplashForm => _splashForm!;

    /// <summary>
    ///   Gets or sets the application load status information
    ///   shown on the splash window.
    /// </summary>
    /// <value>
    ///   The application load status information
    ///   shown on the splash window.
    /// </value>
    /// <remarks>
    ///   For this to work, the splash
    ///   <see cref="Form" /> must implement the
    ///   <see cref="IMessageUpdater" /> interface.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   Form type specified in the
    ///   <see cref="O:Anz.NZInfo.SplashManager.Show">Show</see>
    ///   method
    ///   does not implement the <see cref="IMessageUpdater" /> interface.
    /// </exception>
    public static string Status {
      get => _status!;
      set {
        _status = value;
        if (_splashForm == null) {
          // This could happen if the background thread
          // has not finished instantiating the form.
          // In that case, the background thread
          // will set Status before showing the form.
          return;
        }
        if (!(_splashForm is IMessageUpdater)) {
          throw new InvalidOperationException(
            "Form type \""
            + _splashFormType.Name
            + "\" does not support status messages.");
        }
        try {
          if (_splashForm.InvokeRequired) {
            _splashForm.Invoke(
              new SetMessageDelegate(((IMessageUpdater)_splashForm).SetMessage),
              _status);
          } else {
            ((IMessageUpdater)_splashForm).SetMessage(_status);
          }
        } catch {
          //	fail silently
        }
      }
    }

    /// <summary>
    ///   Gets a value indicating whether the
    ///   Splash <see cref="Form" /> is displayed.
    /// </summary>
    /// <remarks>
    ///   If the background thread has not finished
    ///   showing the splash form, <b>False</b>
    ///   will be returned.
    /// </remarks>
    private static bool Visible => _splashForm != null && _splashForm.Visible;

    /// <summary>
    ///   Closes the Splash <see cref="Form" /> if one is shown.
    /// </summary>
    /// <remarks>
    ///   An exception will <i>not</i> be thrown if no
    ///   splash form is shown.
    /// </remarks>
    public static void Close() {
      if (!_isSplashFormBeingShown) {
        return;
      }
      _isSplashFormBeingShown = false;
      // Ensure that the background thread has finished
      // showing the splash form before attempting to
      // close it.
      while (true) {
        if (_splashThread != null
            && _splashForm != null) {
          break;
        }
        Thread.Sleep(1);
      }
      try {
        _splashForm.Invoke(new MethodInvoker(_splashForm.Close));
      } catch {
        // fail silently
      }
      _splashThread = null;
      _splashForm = null;
    }

    /// <overloads>
    ///   Shows the Splash <see cref="Form" /> in a dedicated thread.
    /// </overloads>
    /// <summary>
    ///   Shows the Splash <see cref="Form" /> in a dedicated thread,
    ///   specifying the splash form type.
    /// </summary>
    /// <param name="splashFormType">The splash form type.</param>
    public static void Show(Type splashFormType) {
      Show(splashFormType, null);
    }

    /// <summary>
    ///   Shows the Splash <see cref="Form" /> in a dedicated thread,
    ///   specifying the splash form type and
    ///   the arguments for the form constructor.
    /// </summary>
    /// <param name="splashFormType">The splash form type.</param>
    /// <param name="args">
    ///   The arguments for the form constructor.
    ///   Can be a null reference (<b>Nothing</b> in Visual Basic).
    /// </param>
    private static void Show(Type splashFormType, params object[]? args) {
      if (_splashThread != null) {
        return;
      }
      if (_splashForm != null) {
        return;
      }
      _isSplashFormBeingShown = true;
      _splashFormType = splashFormType;
      _args = args;
      _splashThread = new Thread(ShowThread) {
        IsBackground = true, Name = nameof(SplashManager)
      };
      _splashThread.Start();
    }

    /// <overloads>
    ///   Shows a message box, putting it in front of the
    ///   splash window if that is showing.
    /// </overloads>
    /// <summary>
    ///   Shows a message box, putting it in front of the
    ///   splash window if that is showing,
    ///   specifying text, buttons and icon.
    /// </summary>
    /// <param name="text">
    ///   The message text.
    /// </param>
    /// <param name="buttons">
    ///   The buttons to be shown on the message box.
    /// </param>
    /// <param name="icon">
    ///   The icon to be shown on the message box.
    /// </param>
    /// <returns>
    ///   The result, if any, returned by the message box.
    /// </returns>
    /// <remarks>
    ///   The message box will be brought into
    ///   the foreground/focus.
    /// </remarks>
    [PublicAPI]
    public static DialogResult ShowMessageBox(
      string text,
      MessageBoxButtons buttons,
      MessageBoxIcon icon) {
      return ShowMessageBox(
        text,
        Application.ProductName,
        buttons,
        icon);
    }

    /// <summary>
    ///   Shows a message box, putting it in front of the
    ///   splash window if that is showing,
    ///   specifying text, caption, buttons and icon.
    /// </summary>
    /// <param name="text">
    ///   The message text.
    /// </param>
    /// <param name="caption">
    ///   The caption to be shown in the message box's
    ///   title bar.
    /// </param>
    /// <param name="buttons">
    ///   The buttons to be shown on the message box.
    /// </param>
    /// <param name="icon">
    ///   The icon to be shown on the message box.
    /// </param>
    /// <returns>
    ///   The result, if any, returned by the message box.
    /// </returns>
    /// <remarks>
    ///   The message box will be brought into
    ///   the foreground/focus.
    /// </remarks>
    [PublicAPI]
    public static DialogResult ShowMessageBox(
      string text,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon) {
      if (Visible) {
        // The splash window is shown on a separate thread.
        // So we need to do a cross-thread invocation.
        if (SplashForm.InvokeRequired) {
          return (DialogResult)SplashForm.Invoke(
            new ShowMessageBoxDelegate(ShowMessageBox), text, caption, buttons,
            icon);
        }
      }
      if (Visible) {
        // This will bring the splash window 
        // and hence the message box into
        // focus.
        SplashForm.Activate();
      }
      return MessageBox.Show(
        SplashForm,
        text,
        caption,
        buttons,
        icon);
    }

    /// <summary>
    ///   Internally used as a thread function - showing the form and
    ///   starting the message loop for it.
    /// </summary>
    private static void ShowThread() {
      try {
        _splashForm = _splashFormType.InvokeMember(
          "Dummy",
          BindingFlags.DeclaredOnly
          | BindingFlags.Public
          | BindingFlags.NonPublic
          | BindingFlags.Instance
          | BindingFlags.CreateInstance,
          null,
          null,
          _args) as Form;
        _args = null;
        if (_status != null) {
          Status = _status;
        }
        Application.Run(_splashForm);
      } catch {
        // fail silently
      }
    }

    private delegate void SetMessageDelegate(string message);

    private delegate DialogResult ShowMessageBoxDelegate(
      string text,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon);
  } // end class
} // end namespace