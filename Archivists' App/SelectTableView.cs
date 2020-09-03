using System;
using System.Windows.Forms;
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
    
    public SelectTableController Controller { get; set; }

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

    public void SetController(SelectTableController controller) {
      Controller = controller;
    }
  } //End of class
} //End of namespace