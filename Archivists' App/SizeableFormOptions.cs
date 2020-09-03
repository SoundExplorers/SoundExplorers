using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers {
  /// <summary>
  ///   Allows a <see cref="Form" />'s size,
  ///   location and state to be saved and restored.
  /// </summary>
  /// <remarks>
  ///   The <see cref="Form" />'s StartPosition
  ///   will be set to <c>Manual</c> so that the settings for
  ///   <see cref="Control.Left" /> and
  ///   <see cref="Control.Top" />
  ///   will take effect.  However, for documentary purposes,
  ///   it is recommended that StartPosition
  ///   still be explicitly set in the calling assembly.
  ///   <para>
  ///     The position (<see cref="Control.Left" /> and
  ///     <see cref="Control.Top" />) of the <see cref="Form" />
  ///     will not be set if it would put the Form outside
  ///     the bounds of the screen's working area.  This could
  ///     happen if the user has switched to a monitor with
  ///     a lower screen resolution, such as a laptop.
  ///   </para>
  ///   <para>
  ///     If a <see cref="Form" />'s details have not
  ///     previously been saved via the <see cref="Save" />
  ///     method:  the default size will be as set
  ///     at design time;  the default position will be
  ///     top left.  If the form is not sizable,
  ///     the default size will always be used.
  ///   </para>
  ///   <para>
  ///     For an MDI child form, only the size and state,
  ///     not the location, will be restored.
  ///     And where this will be a second or subsequent
  ///     MDI child form currently shown within the MDI parent form,
  ///     the size and state will be copied from the
  ///     previously active MDI child form
  ///     instead of being restored from the database.
  ///   </para>
  /// </remarks>
  public class SizeableFormOptions : IView<SizeableFormOptionsController> {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="SizeableFormOptions" /> class,
    ///   restoring the size, position and state
    ///   of the specified <see cref="Form" />
    ///   from the UserOption table.
    /// </summary>
    /// <param name="form">
    ///   The <see cref="Form" /> whose size, position and state are
    ///   to be saved and restored.
    /// </param>
    private SizeableFormOptions([NotNull] Form form) {
      Form = form;
    }

    private SizeableFormOptionsController Controller { get; set; }
    private Form Form { get; }
    private Rectangle InitialScreenBounds { get; set; }

    /// <summary>
    ///   Creates a SizeableFormOptions instance and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    /// </summary>
    /// <param name="form">
    ///   The <see cref="Form" /> whose size, position and state are
    ///   to be saved and restored.
    /// </param>
    [NotNull]
    public static SizeableFormOptions Create([NotNull] Form form) {
      // ViewFactory cannot be used to create the view and controller in this case,
      // as the view constructor requires the form parameter.
      SizeableFormOptions result;
      try {
        result = new SizeableFormOptions(form);
        var dummy =
          new SizeableFormOptionsController(result, form.Name, !form.IsMdiChild);
      } catch (TargetInvocationException ex) {
        throw ex.InnerException ?? ex;
      }
      return result;
    }

    public void SetController(SizeableFormOptionsController controller) {
      Controller = controller;
      Form.StartPosition = FormStartPosition.Manual;
      if (Form.IsMdiChild) {
        SizeChildForm();
      } else {
        PositionParentForm();
      }
    }

    /// <summary>
    ///   Attempts to restore the location, size and state
    ///   of a form other than an MDI child form from the database.
    /// </summary>
    private void PositionParentForm() {
      InitialScreenBounds =
        Screen.GetBounds(new Point(Controller.Left, Controller.Top));
      int x;
      if (Controller.Left >
          InitialScreenBounds.X + InitialScreenBounds.Width
          || Controller.Left < InitialScreenBounds.X) {
        // A screen or area to the right is missing
        // or a screen to the left of the primary screen is missing
        // (X co-ordinates to the left of the primary screen are negative)
        x = 0;
      } else {
        x = Controller.Left;
      }
      int y = Controller.Top >
              InitialScreenBounds.Y + InitialScreenBounds.Height
        // Area to the bottom of the screen is missing
        ? 0
        : Controller.Top;
      if (Controller.Height >= Form.MinimumSize.Height
          && Controller.Height > 0
          && Controller.Width >= Form.MinimumSize.Width
          && Controller.Width > 0) {
        Form.DesktopBounds = new Rectangle(
          x, y, Controller.Width, Controller.Height);
      } else {
        Form.DesktopBounds = new Rectangle(
          x, y, Form.Width, Form.Height);
      }
      // Before a form is displayed, the WindowState property is
      // always set to Normal (0), regardless of its initial setting.
      // So we need to store the UserOption in WindowStateOption and
      // then set Me.WindowState to WindowStateOption.  Otherwise
      // WindowStateOption will always be 0, which would cause the
      // following error:
      // When the window is closed maximised and then reopened, it would
      // be impossible to set the UserOption back to Normal because
      // Form_Unload will not update the UserOption if Me.WindowState
      // = WindowStateOption.
      Form.WindowState = (FormWindowState)Controller.WindowState;
    }

    /// <summary>
    ///   Saves the size, state and, except for MDI child forms,
    ///   location of the <see cref="Form" /> specified in the
    ///   SizeableFormOptions(Form) constructor to the UserOption table.
    /// </summary>
    public void Save() {
      //Debug.WriteLine(Form.Text + " " + Form.WindowState.ToString());
      if (Form.WindowState != FormWindowState.Minimized) {
        Controller.WindowState = (int)Form.WindowState;
      }
      if (!Form.IsMdiChild) {
        // We need to save Left and Top
        // even when the form is maximised.
        // Otherwise, when the main form's position is changed 
        // from maximised on the secondary screen to 
        // maximised on the primary screen, 
        // the new position will not be restored on subsequent load.
        Controller.Left = Form.DesktopBounds.X;
        Controller.Top = Form.DesktopBounds.Y;
      }
      if (Form.WindowState == FormWindowState.Normal) {
        Controller.Height = Form.DesktopBounds.Height;
        Controller.Width = Form.DesktopBounds.Width;
      }
      //if (Form.WindowState == FormWindowState.Maximized
      //&& (   Form.DesktopBounds.X < InitialScreenBounds.X - 100
      //    || Form.DesktopBounds.X > InitialScreenBounds.X + 100)) {
      //    // The form has been maximised onto a different screen
      //    // than it was initially shown on.
      //    // So we have to save the new left.
      //    // Otherwise, next time it is restored,
      //    // it will be maximised onto the wrong screen.
      //    // For unknown reason, if the form is maximised
      //    // onto the screen on which it was originally shown,
      //    // it's X is indicated as four pixels to the left
      //    // of the screen's X.  Y has the same discrepancy.
      //    // In case -4 is not invariant,
      //    // we test for a range + or -100.
      //    // That should be plenty safe because no screen
      //    // is as narrow as 200.
      //    Controller.Left = Form.DesktopBounds.X;
      //}
    }

    /// <summary>
    ///   Copies the size and state of an MDI child form
    ///   from the currently active MDI child form, if any.
    ///   Otherwise attempts to restore the size and state from the database.
    /// </summary>
    private void SizeChildForm() {
      if (Form.MdiParent.MdiChildren.Length > 1) {
        var lastActiveChildForm = Form.MdiParent.ActiveMdiChild ??
                                  throw new NullReferenceException(
                                    nameof(Form.MdiParent.ActiveMdiChild));
        Form.Size = new Size(lastActiveChildForm.Width,
          lastActiveChildForm.Height);
      } else if (Controller.Height >= Form.MinimumSize.Height
                 && Controller.Height > 0
                 && Controller.Width >= Form.MinimumSize.Width
                 && Controller.Width > 0) {
        Form.Size = new Size(Controller.Width, Controller.Height);
        Form.WindowState = (FormWindowState)Controller.WindowState;
      }
    }
  } //End of class
} //End of namespace