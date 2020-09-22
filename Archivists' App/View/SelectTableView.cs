using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers.View {
  /// <summary>
  ///   Select a Table dialog box.
  /// </summary>
  public partial class SelectTableView : Form, IView<SelectTableController> {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="SelectTableView" /> class.
    /// </summary>
    public SelectTableView() {
      InitializeComponent();
      Load += SelectTableView_Load;
    }

    public SelectTableController Controller { get; private set; }

    public void SetController(SelectTableController controller) {
      Controller = controller;
    }

    /// <summary>
    ///   Creates a SelectTableView and its associated controller,
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
    public static SelectTableView Create([NotNull] string tableName) {
      return (SelectTableView)ViewFactory.Create<SelectTableView, SelectTableController>(
        tableName);
    }

    private void OKButton_Click(object sender, EventArgs e) {
      Controller.SelectedEntityListType = (Type)tableComboBox.SelectedValue;
      Controller.SelectedTableName = tableComboBox.Text;
      DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///   Populates the Table ComboBox.
    /// </summary>
    private void PopulateTableComboBox() {
      tableComboBox.DataSource =
        new BindingSource(Controller.EntityListTypeDictionary, null);
      tableComboBox.DisplayMember = "Key";
      tableComboBox.ValueMember = "Value";
    }

    private void SelectTableView_Load(object sender, EventArgs e) {
      PopulateTableComboBox();
      tableComboBox.SelectedValue = Controller.SelectedEntityListType;
    }
  } //End of class
} //End of namespace