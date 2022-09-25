using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SoundExplorers.View; 

/// <summary>
///   A <see cref="Form" />  whose <see cref="Form.Size" />
///   and, optionally, <see cref="Form.Location" />
///   are automatically determined to fit
///   the text that is to be shown on it.
/// </summary>
internal partial class TextViewBase : Form {
  private const int VerticalScrollBarWidth = 16;

  /// <overloads>
  ///   Initialises a new instance of the
  ///   <see cref="TextViewBase" /> class.
  /// </overloads>
  /// <summary>
  ///   A parameterless constructor is required
  ///   for inheritable forms.
  ///   Otherwise the inheriting form fails to
  ///   load in designer view with text
  ///   "Constructor on type '[derived class name]' not found."
  /// </summary>
  protected TextViewBase()
    : this(string.Empty, string.Empty) { }

  /// <summary>
  ///   Initialises a new instance of the
  ///   <see cref="TextViewBase" /> class, specifying
  ///   the text to be shown in the form's
  ///   <see cref="RichTextBox" /> and
  ///   the title to be shown in the form's title bar,
  ///   optionally specifying
  ///   the distance in pixels of the left edge of the dialogue box
  ///   from the left edge of the screen and
  ///   the distance in pixels of the upper edge of the dialog box
  ///   from the top of the screen.
  /// </summary>
  /// <param name="text">
  ///   The text to be shown in the form's
  ///   <see cref="RichTextBox" />.
  /// </param>
  /// <param name="title">
  ///   The title to be shown in the form's title bar.
  /// </param>
  /// <param name="x">
  ///   The distance in pixels of the left edge of the dialogue box
  ///   from the left edge of the screen.  If -1,
  ///   the form will be horizontally centred on the screen.
  /// </param>
  /// <param name="y">
  ///   The distance in pixels of the upper edge of the dialog box
  ///   from the top of the screen.  If -1,
  ///   the form will be vertically centred on the screen.
  /// </param>
  protected TextViewBase(string text, string title, int x = -1, int y = -1) {
    // Required for Windows Form Designer support
    InitializeComponent();
    // Add any constructor code after InitializeComponent call
    MyText = text;
    MyTitle = title;
    X = x;
    Y = y;
  }

  private string MyText { get; }
  private string MyTitle { get; }
  private int X { get; }
  private int Y { get; }

  /// <summary>
  ///   Handles the <see cref="Form" />'s
  ///   <see cref="Form.Load" /> event.
  /// </summary>
  /// <param name="sender">Event sender.</param>
  /// <param name="e">Event arguments.</param>
  private void TextViewBase_Load(object? sender, EventArgs e) {
    if (DesignMode) {
      return;
    }
    // The remainder of this procedure is required
    // at run-time only, not design-time.
    StartPosition = FormStartPosition.Manual;
    Text = MyTitle;
    string text = MyText.TrimEnd(); // Remove any trailing white space

    // What we are trying to achieve:
    //
    //   Make the form as narrow as possible and as short
    //   as possible to show all, or as much as possible, of the
    //   text.
    //
    //   Ensure that the form is no wider or taller than
    //   will fit on the screen without overlapping the task bar.
    //
    //   Do not use a horizontal scroll bar.  Instead, wrap the lines
    //   if required.
    //
    //   Only use a vertical scroll bar
    //   if the line-wrapped text is too tall to show all of it
    //   with the form at its maximum height
    //   for the screen.
    //
    // We are going to show the text in a RichTextBox 
    // instead of a humble TextBox because RichTextBox 
    // has a GetPositionFromCharIndex method,
    // which will help us calculate the 
    // required box dimensions.

    // For the height of a line, we can use the box's
    // PreferredHeight property.  But, for that to work,
    // the Multiline property must be False;
    RichTextBox.Multiline = false;
    int lineHeight = RichTextBox.PreferredHeight;
    // Set Multiline back to True,
    // as we want to show the text in multiple lines.
    RichTextBox.Multiline = true;

    // Assume the RichTextBox's minimum size
    // is as at design time of the derived form.
    var minSize = new Size(RichTextBox.Width, RichTextBox.Height);
    // Similarly, we can find out the RichTextBox's
    // maximum size by making the form as big as will fit on the
    // screen without overlapping the task bar.
    // Note that maximising the form does not have the same affect
    // in this context:  it does not alter the form's Width or Height
    // properties.
    // The form will be shown on the screen of the owner form,
    // if specified, otherwise on the primary screen.
    var screen = Owner != null
      ? Screen.FromPoint(Owner.Location)
      : Screen.PrimaryScreen;
    //Debug.WriteLine(screen.WorkingArea.Left);
    //Debug.WriteLine(screen.WorkingArea.Top);
    Height = screen.WorkingArea.Height;
    Width = screen.WorkingArea.Width;
    var maxSize = new Size(RichTextBox.Width, RichTextBox.Height);

    // Work out the height required for the RichTextBox
    // and whether we need a vertical scroll bar:
    // see how far down the box the text would go in word-wrap mode 
    // and without a vertical scroll bar
    // if the box were as wide as possible.
    RichTextBox.ScrollBars = RichTextBoxScrollBars.None;
    RichTextBox.WordWrap = true;
    RichTextBox.Text = text;
    // Even if the text is too long for the box's maximum height,
    // GetPositionFromCharIndex will still return a valid
    // notional position for the end of the text.
    // And, yes, it is possible to set
    // GetPositionFromCharIndex's index argument to
    // one after the last character, i.e. to the end
    // of the text.
    // Because will GetPositionFromCharIndex return the position
    // of the top of the last character, we need to add the
    // height of a line to get the required box height.
    var boxSize = new Size(
      minSize.Width,
      RichTextBox.GetPositionFromCharIndex(RichTextBox.Text.Length).Y +
      lineHeight);
    // Make sure the box is at least as tall as the minimum
    // height and no taller than the maximum height.
    // If the text is too long for the box's maximum height,
    // use a vertical scroll bar.
    if (boxSize.Height < minSize.Height) {
      boxSize.Height = minSize.Height;
    } else if (boxSize.Height > maxSize.Height) {
      boxSize.Height = maxSize.Height;
      RichTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
    }

    // Turn word-wrap mode off so that we can try out
    // each line of the text in turn as a single-line
    // text in the box.  Then we can see how wide
    // each line would need to be.
    // The required box width is the width of the longest
    // line or the maximum width, whichever is narrower.
    RichTextBox.WordWrap = false;
    var reader = new StringReader(text);
    while (reader.Peek() >= 0) {
      string line = reader.ReadLine()!;
      // Evidently we need to add an extra character to calculate
      // a line width that is safely big enough.  
      // Presumably this is to allow
      // for the box's border and/or margins.
      RichTextBox.Text = line + "X";
      int lineWidth =
        RichTextBox.GetPositionFromCharIndex(RichTextBox.Text.Length).X;
      if (RichTextBox.ScrollBars == RichTextBoxScrollBars.Vertical) {
        lineWidth += VerticalScrollBarWidth;
      }
      if (lineWidth >= maxSize.Width) {
        boxSize.Width = maxSize.Width;
        break;
      }
      if (lineWidth > boxSize.Width) {
        boxSize.Width = lineWidth;
      }
    }
    reader.Close();

    // We have finished using the box to measure
    // line widths.  So set the box's text and
    // wrap mode to what we are actually going to show.
    RichTextBox.WordWrap = true;
    RichTextBox.Text = text;

    // Because the box is docked,
    // this is how we adjust the form and the box to their
    // required sizes.
    Height = Height - RichTextBox.Height + boxSize.Height;
    Width = Width - RichTextBox.Width + boxSize.Width;

    // Position the form in the centre of the screen's
    // working area unless otherwise specified.
    if (X == -1) {
      Left =
        screen.WorkingArea.Left
        + (screen.WorkingArea.Width
           - Width) / 2;
    } else {
      Left = X;
    }
    if (Y == -1) {
      Top =
        screen.WorkingArea.Top
        + (screen.WorkingArea.Height
           - Height) / 2;
    } else {
      Top = Y;
    }
  }
}