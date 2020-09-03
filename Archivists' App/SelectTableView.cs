using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SoundExplorers.Controller;

namespace SoundExplorers {
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

    public void SetController(SelectTableController controller) {
      Controller = controller;
    }

    private void OKButton_Click(object sender, EventArgs e) {
      Controller.TableName = tableComboBox.Text;
      DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///   Populates the Table ComboBox.
    /// </summary>
    private void PopulateTableComboBox() {
      tableComboBox.DataSource =
        new BindingSource(Controller.EntityListTypes, null);
      tableComboBox.DisplayMember = "Key";
      tableComboBox.ValueMember = "Key";
    }

    private void SelectTableView_Load(object sender, EventArgs e) {
      PopulateTableComboBox();
      tableComboBox.SelectedIndex =
        tableComboBox.FindStringExact(Controller.TableName);
    }
  } //End of class
} //End of namespace