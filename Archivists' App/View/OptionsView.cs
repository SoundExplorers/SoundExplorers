using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  internal partial class OptionsView : Form, IView<OptionsController> {
    public OptionsView() {
      InitializeComponent();
      DatabaseFolderTextBox.ContextMenuStrip =
        new TextBoxContextMenu(DatabaseFolderTextBox);
    }

    private OptionsController Controller { get; set; }

    public void SetController(OptionsController controller) {
      Controller = controller;
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

    private void BrowseButton_Click(object sender, EventArgs e) {
      // var openFileDialog = new OpenFileDialog();
      // openFileDialog.CheckFileExists = true;
      // openFileDialog.InitialDirectory = Controller.DatabaseFolderPath;
      var folderPicker = new CommonOpenFileDialog {
        Title = "Select the Database Folder",
        IsFolderPicker = true,
        InitialDirectory = Controller.DatabaseFolderPath,
      };
      if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok) {
        DatabaseFolderTextBox.Text = folderPicker.FileName;
      }
    }

    private void OkButton_Click(object sender, EventArgs e) {
      Controller.DatabaseFolderPath = DatabaseFolderTextBox.Text;
      DialogResult = DialogResult.OK;
    }
  }
}