using System;
using System.IO;
using System.Windows.Forms;

namespace SoundExplorers {

    /// <summary>
    /// An open file dialogue for which the user's preference for
    /// folder path and optionally file name are stored via a 
    /// <see cref="UserOptionCollection"/>.
    /// </summary>
    /// <remarks>
    /// Derived classes may override the 
    /// <see cref="UserOptionFileDialog.GetOption"/> 
    /// and <see cref="UserOptionFileDialog.SetOption"/> 
    /// methods to
    /// restore and save the folder path and file name
    /// elswhere than via a 
    /// <see cref="UserOptionCollection"/>.
    /// </remarks>
    public class UserOptionOpenFileDialog : UserOptionFileDialog {

        #region Fields
        private string _fileName = "";
        private string _fileNameUserOptionName = "";
        private string _oldFileName = "";
        #endregion Fields

        #region Constructors
        /// <overloads>
        /// Initialises a new instance of the 
        /// <see cref="UserOptionOpenFileDialog"/> class.
        /// </overloads>
        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="UserOptionOpenFileDialog"/> class,
        /// specifying the dialogue box's title and the UserOption
        /// containing the path of the initial folder.
        /// </summary>
        /// <param name="dialogTitle">
        /// The dialogue box's title.
        /// </param>
        /// <param name="folderPathUserOptionName">
        /// The name of the user option whose value is 
        /// the path of the initial folder.
        /// </param>
        /// <remarks>
        /// The constructor retrieves the user option, if found,
        /// whose name is specified in 
        /// <paramref name="folderPathUserOptionName"/>.
        /// (If a folder user option is found,
        /// an existence check for the folder specified in
        /// the user option is not done till
        /// the <see cref="UserOptionFileDialog.GetPath"/> method is called.)
        /// <para>
        /// The filter is defaulted to Excel (*.xls).
        /// </para>
        /// </remarks>
        public UserOptionOpenFileDialog(
                string dialogTitle,
                string folderPathUserOptionName) :
            this(
                dialogTitle,
                folderPathUserOptionName,
                "",
                "Excel",
                "xls") {
        }

        /// <summary>
        /// Initialises a new instance of the 
        /// <see cref="UserOptionOpenFileDialog"/> class,
        /// specifying the dialogue box's title and the UserOptions
        /// containing the path of the initial folder 
        /// and the initial file name.
        /// </summary>
        /// <param name="dialogTitle">
        /// The dialogue box's title.
        /// </param>
        /// <param name="folderPathUserOptionName">
        /// The name of the user option whose value is 
        /// the path of the initial folder.
        /// </param>
        /// <param name="fileNameUserOptionName">
        /// The name of the user option whose value is 
        /// the the initial file name.
        /// A null reference or an empty string
        /// if an initial file is not required.
        /// </param>
        /// <remarks>
        /// The constructor retrieves the user option, if found,
        /// whose name is specified in 
        /// <paramref name="folderPathUserOptionName"/> and 
        /// the user option, if found,
        /// whose name is specified in 
        /// <paramref name="fileNameUserOptionName"/>.
        /// (If folder and file user options are found,
        /// existence checks for the folder and file specified in
        /// the user options are not done till
        /// the <see cref="UserOptionFileDialog.GetPath"/> method is called.)
        /// <para>
        /// The filter is defaulted to Excel (*.xls).
        /// </para>
        /// </remarks>
        public UserOptionOpenFileDialog(
                string dialogTitle,
                string folderPathUserOptionName,
                string fileNameUserOptionName) :
            this(
                dialogTitle,
                folderPathUserOptionName,
                fileNameUserOptionName,
                "Excel",
                "xls") {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UserOptionOpenFileDialog"/> class,
        /// specifying the dialogue box's title,
        /// the filter and the UserOptions
        /// containing the path of the initial folder 
        /// and the initial file name.
        /// </summary>
        /// <param name="dialogTitle">
        /// The dialogue box's title.
        /// </param>
        /// <param name="folderPathUserOptionName">
        /// The name of the user option whose value is 
        /// the path of the initial folder.
        /// </param>
        /// <param name="fileNameUserOptionName">
        /// The name of the user option whose value is 
        /// the the initial file name.
        /// A null reference or an empty string
        /// if an initial file is not required.
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
        /// <paramref name="folderPathUserOptionName"/> and 
        /// the user option, if found,
        /// whose name is specified in 
        /// <paramref name="fileNameUserOptionName"/>.
        /// (If folder and file user options are found,
        /// existence checks for the folder and file specified in
        /// the user options are not done till
        /// the <see cref="UserOptionFileDialog.GetPath"/> method is called.)
        /// </remarks>
        public UserOptionOpenFileDialog(
                string dialogTitle,
                string folderPathUserOptionName,
                string fileNameUserOptionName,
                string filterName,
                string filterExtension)  :
            base(
                new OpenFileDialog(),
                dialogTitle,
                folderPathUserOptionName,
                filterName,
                filterExtension) {
            _fileNameUserOptionName = fileNameUserOptionName;
            if (_fileNameUserOptionName != null
            && _fileNameUserOptionName.Length != 0) {
                _oldFileName = base.GetOption(_fileNameUserOptionName);
                _fileName = _oldFileName;
            }
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
        /// If a user option for the initial file
        /// was found by the 
        /// <see cref="O:Anz.NZInfo.UserOptionFileDialog.#ctor">
        /// constructor</see>
        /// and the file specified by the user option exists
        /// in the specified initial folder,
        /// the initial file name of the dialogue 
        /// will be set to that file.
        /// <para>
        /// If a user option for the initial folder is not found,
        /// or the folder specified by the user option does not exist,
        /// the dialogue box's folder will be intially set to
        /// the user's My Documents folder.
        /// </para>
        /// </remarks>
        public override string GetPath() {
            if (FolderPath == "") {
                this.FileDialog.FileName = "";
            } else if (Directory.Exists(FolderPath)) {
                if (_fileName.Length != 0) {
                    if (File.Exists(FolderPath + @"\" + _fileName)) {
                        this.FileDialog.FileName = _fileName;
                    } else {
                        this.FileDialog.FileName = "";
                    }
                } else {
                    this.FileDialog.FileName = "";
                }
            } else {// Folder not found
                this.FileDialog.FileName = "";
            }
            if (base.GetPath() == "") {
                return "";
            }
            _fileName = Path.GetFileName(this.FileDialog.FileName);
            return this.FileDialog.FileName;
        }//End of GetPath

        /// <summary>
        /// Saves the dialogue's folder path, if it has changed,
        /// to the user option whose name was specified in the
        /// folderPathUserOptionName parameter of the constructor.
        /// If the fileNameUserOptionName parameter of the constructor
        /// was specified,
        /// saves the dialogue's file name, if it has changed,
        /// to the user option whose name was specified in the
        /// fileNameUserOptionName parameter of the constructor.
        /// </summary>
        /// <remarks>
        /// If the <see cref="UserOptionCollection"/>
        /// was specified via the
        /// <see cref="O:Anz.NZInfo.UserOptionOpenFileDialog.#ctor">
        /// constructor</see>,
        /// the calling assembly is responsible for saving
        /// the user options by calling 
        /// <see cref="UserOptionCollection.Dispose">
        /// UserOptionCollection.Dispose</see>
        /// after calling <b>UserOptionOpenFileDialog.SaveOptions</b>.
        /// </remarks>
        public override void SaveOptions() {
            base.SaveOptions();
            if (_fileNameUserOptionName != null
            && _fileNameUserOptionName.Length != 0) {
                if (_fileName != _oldFileName) {
                    base.SetOption(_fileNameUserOptionName, _fileName);
                }
            }
        }//End of SaveOptions
        #endregion Methods
    }//End of class
}//End of namespace
