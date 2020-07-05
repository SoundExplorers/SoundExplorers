using System;
using System.IO;
using System.Windows.Forms;
using SoundExplorers.Data;

namespace SoundExplorers {

    /// <summary>
    /// A file dialogue for which the user's preference for
    /// folder path is stored via a 
    /// <see cref="UserOptionCollection"/>.
    /// </summary>
    /// <remarks>
    /// Derived classes may override the 
    /// <see cref="UserOptionFileDialog.GetOption"/> 
    /// and <see cref="UserOptionFileDialog.SetOption"/> 
    /// methods to
    /// restore and save the folder path
    /// elswhere than via a 
    /// <see cref="UserOptionCollection"/>.
    /// </remarks>
    public abstract class UserOptionFileDialog {

        #region Fields
        private string _folderPathUserOptionName = "";
        private string _oldFolderPath = "";
        #endregion Fields

        #region Properties
        /// <summary>
        /// The dialogue box to be shown.
        /// </summary>
        protected FileDialog FileDialog { get; private set; }

        /// <summary>
        /// The folder path, either when the dialogue box
        /// is shown or of the selected file.
        /// </summary>
        protected string FolderPath { get; private set; }
        #endregion Properties

        #region Constructors
        /// <overloads>
        /// Initialises a new instance of the 
        /// <see cref="UserOptionFileDialog"/> class.
        /// </overloads>
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="UserOptionFileDialog"/> class,
        /// specifying the dialogue box to be shown, 
        /// the dialogue box's title,
        /// the filter and the UserOption
        /// containing the path of the initial folder.
        /// </summary>
        /// <param name="fileDialog">
        /// The dialogue box to be shown.
        /// </param>
        /// <param name="dialogTitle">
        /// The dialogue box's title.
        /// </param>
        /// <param name="folderPathUserOptionName">
        /// The name to be appended to the entry assembly name prefix
        /// to specify the name of the user option whose value is 
        /// the path of the initial folder.
        /// </param>
        /// <param name="filterName">
        /// The filter's name.
        /// </param>
        /// <param name="filterExtension">
        /// The filter's extension, excluding the initial ".".
        /// </param>
        /// <remarks>
        /// The constructor retrieves the user option, if found,
        /// whose name is specified in 
        /// <paramref name="folderPathUserOptionName"/>.
        /// </remarks>
        protected UserOptionFileDialog(
                FileDialog fileDialog,
                string dialogTitle,
                string folderPathUserOptionName,
                string filterName,
                string filterExtension) {
            this.FileDialog = fileDialog;
            _folderPathUserOptionName = folderPathUserOptionName;
            this.FileDialog.DefaultExt = "." + filterExtension;
            this.FileDialog.Title = dialogTitle;
            this.FileDialog.Filter =
                filterName + " (*." + filterExtension + ")|*" + filterExtension;
            _oldFolderPath = GetOption(_folderPathUserOptionName);
            FolderPath = _oldFolderPath;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Shows the dialogue box and returns the path
        /// of the file selected.
        /// </summary>
        /// <returns>
        /// If the dialogue is cancelled, a zero-length string.
        /// Otherwise the path of the selected file.
        /// </returns>
        /// <remarks>
        /// If a user option for the initial folder
        /// was found by the 
        /// <see cref="O:Anz.NZInfo.UserOptionFileDialog.#ctor">
        /// constructor</see>
        /// and the folder specified by the user option exists,
        /// the initial folder path of the dialogue 
        /// will be set to that folder.
        /// <para>
        /// If a user option for the initial folder is not found,
        /// or the folder specified by the user option does not exist,
        /// the dialogue box's folder will be intially set to
        /// the user's My Documents folder.
        /// </para>
        /// </remarks>
        public virtual string GetPath() {
            if (FolderPath == "") {
                this.FileDialog.InitialDirectory =
                  Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            } else if (Directory.Exists(FolderPath)) {
                this.FileDialog.InitialDirectory = FolderPath;
            } else {// Folder not found
                this.FileDialog.InitialDirectory =
                  Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            if (this.FileDialog.ShowDialog() == DialogResult.Cancel) {
                return "";
            }
            FolderPath = Path.GetDirectoryName(this.FileDialog.FileName);
            return this.FileDialog.FileName;
        }//End of GetPath

        /// <summary>
        /// Saves the dialogue's folder path, if it has changed,
        /// to the user option whose name was specified in the
        /// folderPathUserOptionName parameter of the constructor.
        /// </summary>
        public virtual void SaveOptions() {
            if (_folderPathUserOptionName.Length != 0) {
                // Provided the Initialise method has been called,
                // the name of folder path user option will have
                // been specified.  So checking that it is non-blank
                // is just a precaution.
                if (FolderPath != _oldFolderPath) {
                    SetOption(_folderPathUserOptionName, FolderPath);
                }
            }
        }//End of SaveOptions

        /// <summary>
        /// Gets the current value of a user option, 
        /// defaulting to an empty string.
        /// </summary>
        /// <param name="key">
        /// The key identifying the option.
        /// </param>
        /// <returns>
        /// The current value of the option, if found. 
        /// Otherwise an empty string.
        /// </returns>
        protected virtual string GetOption(string key) {
            var userOption = new UserOption();
            userOption.OptionName = key;
            if (userOption.Fetch()) {
                return userOption.OptionValue;
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets a value for a user option.
        /// </summary>
        /// <param name="key">
        /// The key identifying the option.
        /// </param>
        /// <param name="value">
        /// The value to which the option is to be set.
        /// </param>
        protected virtual void SetOption(string key, string value) {
            var userOption = new UserOption();
            userOption.OptionName = key;
            userOption.OptionValue = value;
            userOption.Save();
        }
        #endregion Methods
    }//End of class
}//End of namespace