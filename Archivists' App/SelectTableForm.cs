using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SoundExplorers.Data;

namespace SoundExplorers {

    /// <summary>
    /// Select a Table dialogue box.
    /// </summary>
    public partial class SelectTableForm : Form {

        #region Public Properties
        /// <summary>
        /// Gets the name table whose
        /// data is to be displayed.
        /// </summary>
        public string TableName { get; private set; }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="SelectTableForm"/> class.
        /// </summary>
        /// <param name="tableName">
        /// The name of the table that is to be initially selected.
        /// An emtpy string for no table to be initially selected.
        /// </param>
        public SelectTableForm(string tableName) {
            InitializeComponent();
            TableName = tableName;
            PopulateTableComboBox();
            this.tableComboBox.SelectedIndex =
                this.tableComboBox.FindStringExact(TableName);
        }
        #endregion Constructors

        #region Event Handlers
        private void OKButton_Click(object sender, EventArgs e) {
            TableName = this.tableComboBox.Text;
            this.DialogResult = DialogResult.OK;
        }
        #endregion Event Handlers

        #region Private Methods
        /// <summary>
        /// Populates the Table ComboBox.
        /// </summary>
        private void PopulateTableComboBox() {
            this.tableComboBox.DataSource =
                new BindingSource(Factory<IEntityList>.Types, null);
            //new BindingSource(Factory<IEntity>.Types, null);
            //new BindingSource(Factory<IEntityList>.Types, null);
            this.tableComboBox.DisplayMember = "Key";
            this.tableComboBox.ValueMember = "Key";
        }
        #endregion Private Methods
    }//End of class
}//End of namespace