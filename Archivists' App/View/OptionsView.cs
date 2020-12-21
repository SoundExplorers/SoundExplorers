using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal partial class OptionsView : Form, IView<OptionsController> {
    public OptionsView() {
      InitializeComponent();
      DatabaseFolderTextBox.ContextMenuStrip =
        new TextBoxContextMenu(DatabaseFolderTextBox);
      DatabaseFolderTextBox.MouseDown += TextBox_MouseDown;
      MessageTextBox.ContextMenuStrip =
        new TextBoxContextMenu(MessageTextBox);
      MessageTextBox.MouseDown += TextBox_MouseDown;
    }

    private OptionsController Controller { get; set; }

    public void SetController(OptionsController controller) {
      Controller = controller;
      Controller.LoadDatabaseConfig();
      DatabaseFolderTextBox.Text = Controller.DatabaseFolderPath;
      MessageTextBox.Text = Controller.Message;
    }

    /// <summary>
    ///   Creates a OptionsView and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    /// </summary>
    [NotNull]
    public static OptionsView Create() {
      return (OptionsView)ViewFactory.Create<OptionsView, OptionsController>();
    }

    /// <summary>
    ///   Allows right-clicking the TextBox to focus it and then show its context menu.
    /// </summary>
    private static void TextBox_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        (sender as TextBox)?.Focus();
      }
    }
  }
}