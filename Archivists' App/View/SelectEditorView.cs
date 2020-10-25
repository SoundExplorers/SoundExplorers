using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   Select a Editor dialog box.
  /// </summary>
  public partial class SelectEditorView : Form, IView<SelectEditorController> {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="SelectEditorView" /> class.
    /// </summary>
    public SelectEditorView() {
      InitializeComponent();
      Load += SelectEditorView_Load;
    }

    public SelectEditorController Controller { get; private set; }

    public void SetController(SelectEditorController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Creates a SelectEditorView and its associated controller,
    ///   as per the Model-View-Controller design pattern,
    ///   returning the view instance created.
    ///   The parameter is passed to the controller's constructor.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table that is to be initially selected.
    ///   An empty string for no table to be initially selected.
    /// </param>
    /// <returns></returns>
    [NotNull]
    public static SelectEditorView Create([NotNull] string tableName) {
      return (SelectEditorView)ViewFactory.Create<SelectEditorView, SelectEditorController>(
        tableName);
    }

    private void OKButton_Click(object sender, EventArgs e) {
      Controller.SelectedEntityListType = (Type)tableComboBox.SelectedValue;
      Controller.SelectedTableName = tableComboBox.Text;
      DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///   Populates the Editor ComboBox.
    /// </summary>
    private void PopulateEditorComboBox() {
      tableComboBox.DataSource =
        new BindingSource(Controller.EntityListTypeDictionary, null);
      tableComboBox.DisplayMember = "Key";
      tableComboBox.ValueMember = "Value";
    }

    private void SelectEditorView_Load(object sender, EventArgs e) {
      PopulateEditorComboBox();
      if (Controller.SelectedEntityListType != null) {
        tableComboBox.SelectedValue = Controller.SelectedEntityListType;
      }
    }
  } //End of class
} //End of namespace