using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal partial class OptionsView : Form, IView<OptionsController> {
    public OptionsView() {
      InitializeComponent();
      DatabaseFolderTextBox.ContextMenuStrip =
        new TextBoxContextMenu(DatabaseFolderTextBox);
      MessageTextBox.ContextMenuStrip =
        new TextBoxContextMenu(MessageTextBox);
    }

    private OptionsController Controller { get; set; } = null!;

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
  }
}