using System;
using System.Data;
using System.IO;
using Devart.Data.PostgreSql;

namespace SoundExplorers.Data {
    /// <summary>
    ///   Image entity.
    /// </summary>
    internal class Image : Entity<Image> {
    private static DirectoryInfo _defaultFolder;
    private static Option _defaultFolderOption;
    private static PgSqlCommand _selectNextImageIdCommand;
    [Field] public string Comments { get; set; }
    [ReferencedField("Performance.Date")] public DateTime Date { get; set; }

    /// <summary>
    ///   Gets or sets a default folder to be used,
    ///   if the folder of the path (if any) currently in
    ///   the <see cref="Path" /> property
    ///   is not specified or does not exist,
    ///   as the initial folder for the Open dialogue that may be
    ///   shown to select a file to update the path in
    ///   the <see cref="Path" /> property.
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
              Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
          }
        }
        return _defaultFolder;
      }
      set {
        _defaultFolder = value;
        DefaultFolderOption.StringValue = value.FullName;
      }
    }

    private static Option DefaultFolderOption {
      get {
        if (_defaultFolderOption == null) {
          _defaultFolderOption = new Option(
            "Image.DefaultFolder");
        }
        return _defaultFolderOption;
      }
    }

    /// <summary>
    ///   Gets whether the file, if any, specified by
    ///   the <see cref="Path" /> field propery exists.
    ///   False if a path is not specified or the file does not exist.
    /// </summary>
    public virtual bool FileExists =>
      !string.IsNullOrEmpty(Path)
      && File.Exists(Path);

    [PrimaryKeyField] [HiddenField] public int ImageId { get; set; }
    [ReferencedField("Name")] public string Location { get; set; }
    [UniqueKeyField] public string Path { get; set; }

    /// <summary>
    ///   Gets an SQL command to select the next value in the ImageId column's sequence.
    /// </summary>
    private static PgSqlCommand SelectNextImageIdCommand {
      get {
        if (_selectNextImageIdCommand == null) {
          _selectNextImageIdCommand = new PgSqlCommand(
            SqlHelper.GetSql("Select Next ImageId.sql"),
            new PgSqlConnection(
              SqlHelper.ConnectionString));
          try {
            _selectNextImageIdCommand.Connection.Open();
            _selectNextImageIdCommand.Prepare();
          } catch (PgSqlException ex) {
            throw new DataException(
              "Error on preparing SQL command:" + Environment.NewLine
                                                + ex.Message +
                                                Environment.NewLine +
                                                Environment.NewLine
                                                + "SQL command text:" +
                                                Environment.NewLine +
                                                Environment.NewLine
                                                + _selectNextImageIdCommand
                                                  .CommandText
                                                + Environment.NewLine +
                                                Environment.NewLine,
              ex);
          } finally {
            _selectNextImageIdCommand.Connection.Close();
          }
        }
        return _selectNextImageIdCommand;
      }
    }

    [Field] public string Title { get; set; }

    /// <summary>
    ///   Fetches the Newsletter (i.e. not just the Newsletter date)
    ///   for the Image's Performance from the database.
    /// </summary>
    /// <returns>
    ///   The Newsletter for the Image's Performance.
    /// </returns>
    public virtual Newsletter FetchNewsletter() {
      var performance = new Performance();
      performance.Location = Location;
      performance.Date = Date;
      performance.Fetch();
      return performance.FetchNewsletter();
    }

    /// <summary>
    ///   Fetches the next value in the ImageId column's sequence
    ///   from the database.
    /// </summary>
    /// <returns>
    ///   The next value in the ImageId column's sequence.
    /// </returns>
    /// <remarks>
    ///   ImageId.ImageId should be set to this
    ///   when inserting a new row into the Image table.
    /// </remarks>
    public static int GetNextImageId() {
      try {
        SelectNextImageIdCommand.Connection.Open();
        return Convert.ToInt32(SelectNextImageIdCommand.ExecuteScalar());
      } catch (PgSqlException ex) {
        throw new DataException(
          "Error on executing SQL command:" + Environment.NewLine
                                            + ex.Message + Environment.NewLine +
                                            Environment.NewLine
                                            + "SQL command text:" +
                                            Environment.NewLine +
                                            Environment.NewLine
                                            + SelectNextImageIdCommand
                                              .CommandText
                                            + Environment.NewLine +
                                            Environment.NewLine,
          ex);
      } finally {
        SelectNextImageIdCommand.Connection.Close();
      }
    }
  } //End of class
} //End of namespace