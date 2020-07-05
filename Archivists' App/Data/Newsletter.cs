using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    /// <summary>
    /// Newsletter entity.
    /// </summary>
    internal class Newsletter : PieceOwningEntity<Newsletter> {

        #region Private Fields
        private static DirectoryInfo _defaultFolder;
        private static Option _defaultFolderOption;
        #endregion Private Fields

        #region Public Field Properties
        [PrimaryKeyField]
        public DateTime Date { get; set; }

        [UniqueKeyField]
        public string Path { get; set; }
        #endregion Public Field Properties

        #region Other Public Properties
        /// <summary>
        /// Gets or sets a default folder to be used,
        /// if the folder of the path (if any) currently in 
        /// the <see cref="Path"/> property
        /// is not specified or does not exist,
        /// as the initial folder for the Open dialogue that may be
        /// shown to select a file to update the path in 
        /// the <see cref="Path"/> property.
        /// </summary>
        public static DirectoryInfo DefaultFolder {
            get {
                if (_defaultFolder == null) {
                    if (DefaultFolderOption.StringValue != string.Empty) {
                        _defaultFolder = new DirectoryInfo(DefaultFolderOption.StringValue);
                    }
                    if (_defaultFolder == null
                    || !_defaultFolder.Exists) {
                        _defaultFolder = new DirectoryInfo(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    }
                }
                return _defaultFolder;
            }
            set {
                _defaultFolder = value;
                DefaultFolderOption.StringValue = value.FullName;
            }
        }
        #endregion Other Public Properties

        #region Private Properties
        private static Option DefaultFolderOption {
            get {
                if (_defaultFolderOption == null) {
                    _defaultFolderOption = new Option(
                       "Newsletter.DefaultFolder");
                }
                return _defaultFolderOption;
            }
        }
        #endregion Private Properties
    }//End of class
}//End of namespace
