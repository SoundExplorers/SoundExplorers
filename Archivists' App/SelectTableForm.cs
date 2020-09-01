using System;
using System.Windows.Forms;

namespace SoundExplorers {
  /// <summary>
  ///   Select a Table dialogue box.
  /// </summary>
  public partial class SelectTableForm : Form {
    /// <summary>
    ///   Initialises a new instance of the
    ///   <see cref="SelectTableForm" /> class.
    /// </summary>
    /// <param name="tableName">
    ///   The name of the table that is to be initially selected.
    ///   An emtpy string for no table to be initially selected.
    /// </param>
    public SelectTableForm(string tableName) {
      InitializeComponent();
      TableName = tableName;
      PopulateTableComboBox();
      tableComboBox.SelectedIndex =
        tableComboBox.FindStringExact(TableName);
    }

    /// <summary>
    ///   Gets the name table whose
    ///   data is to be displayed.
    /// </summary>
    public string TableName { get; private set; }

    private void OKButton_Click(object sender, EventArgs e) {
      TableName = tableComboBox.Text;
      DialogResult = DialogResult.OK;
    }

    /// <summary>
    ///   Populates the Table ComboBox.
    /// </summary>
    private void PopulateTableComboBox() {
      tableComboBox.DataSource =
        new BindingSource(Factory<IEntityList>.Types, null);
      //new BindingSource(Factory<IEntity>.Types, null);
      //new BindingSource(Factory<IEntityList>.Types, null);
      tableComboBox.DisplayMember = "Key";
      tableComboBox.ValueMember = "Key";
    }
  } //End of class
} //End of namespace