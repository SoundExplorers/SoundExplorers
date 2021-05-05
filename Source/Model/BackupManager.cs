using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public class BackupManager : IBackupManager {
    private GlobalOption? _backupFolderPathOption;
    private GlobalOption? _lastBackupDateTimeOption;
    private GlobalOption? _lastPromptForBackupDateTimeOption;

    public BackupManager(QueryHelper? queryHelper, SessionBase? session) {
      QueryHelper = queryHelper ?? QueryHelper.Instance;
      Session = session ?? Global.Session;
    }

    private GlobalOption BackupFolderPathOption =>
      _backupFolderPathOption ??=
        new GlobalOption(QueryHelper, Session, nameof(BackupFolderPath));

    private GlobalOption LastBackupDateTimeOption =>
      _lastBackupDateTimeOption ??=
        new GlobalOption(QueryHelper, Session, nameof(LastBackupDateTime));

    private DateTime LastBackupCheckDateTime {
      get => LastBackupCheckDateTimeOption.DateTimeValue;
      set => LastBackupCheckDateTimeOption.DateTimeValue = value;
    }

    private GlobalOption LastBackupCheckDateTimeOption =>
      _lastPromptForBackupDateTimeOption ??=
        new GlobalOption(QueryHelper, Session, nameof(LastBackupCheckDateTime));

    private QueryHelper QueryHelper { get; }
    private SessionBase Session { get; }

    public string BackupFolderPath {
      get => BackupFolderPathOption.StringValue;
      private set => BackupFolderPathOption.StringValue = value;
    }

    public bool IsTimeToPromptForBackup {
      get {
        if (LastBackupDateTime.AddDays(7) < DateTime.Now &&
            LastBackupCheckDateTime.AddDays(7) < DateTime.Now) {
          LastBackupCheckDateTime = DateTime.Now;
          return true;
        }
        return false;
      }
    }

    public DateTime LastBackupDateTime {
      get => LastBackupDateTimeOption.DateTimeValue;
      private set => LastBackupDateTimeOption.DateTimeValue = value;
    }

    public string PromptForBackupQuestion {
      get {
        var writer = new StringWriter();
        writer.Write("Would you like to back up the database now?");
        if (LastBackupDateTime > DateTime.MinValue) {
          writer.WriteLine();
          writer.WriteLine();
          writer.Write("The database was last backed up on ");
          writer.Write(
            $"{LastBackupDateTime:dd MMMM yyyy} at {LastBackupDateTime:HH:mm:ss}.");
        }
        return writer.ToString();
      }
    }

    public void BackupDatabaseTo(string backupFolderPath) {
      if (!Directory.Exists(backupFolderPath)) {
        throw new ApplicationException(
          $"Backup folder '{backupFolderPath}' does not exist.");
      }
      var backupDateTime = GetBackupDateTime();
      string zipFileName = Path.Combine(backupFolderPath,
        $"Backup{backupDateTime:yyyyMMddHHmmss}.zip");
      ZipFolder(Session.SystemDirectory, zipFileName);
      BackupFolderPath = backupFolderPath;
      LastBackupDateTime = backupDateTime;
    }

    /// <summary>
    ///   Extracts the files from the specified zip file into the specified folder.
    /// </summary>
    /// <remarks>
    ///   Based on
    ///   https://ourcodeworld.com/articles/read/629/how-to-create-and-extract-zip-files-compress-and-decompress-zip-with-sharpziplib-with-csharp-in-winforms.
    /// </remarks>
    internal static void UnzipToFolder(string zipFilePath, string outputFolderPath) {
      ZipFile? zipFile = null;
      try {
        FileStream fileStream = File.OpenRead(zipFilePath);
        zipFile = new ZipFile(fileStream);
        var zippedFileEntries =
          from ZipEntry zipEntry in zipFile where zipEntry.IsFile select zipEntry;
        foreach (ZipEntry zippedFileEntry in zippedFileEntries) {
          string entryFileName = zippedFileEntry.Name;
          // To remove the folder from the entry:
          //   entryFileName = Path.GetFileName(entryFileName);
          // Optionally match entry names against a selection list here to skip as
          // desired.
          // The unpacked length is available in the zipEntry.Size property.
          // 4K is optimum
          byte[] buffer = new byte[4096];
          Stream zipStream = zipFile.GetInputStream(zippedFileEntry);
          // Manipulate the output filename here as desired.
          string fullZipToPath = Path.Combine(outputFolderPath, entryFileName);
          string directoryName = Path.GetDirectoryName(fullZipToPath)!;
          if (directoryName.Length > 0) {
            Directory.CreateDirectory(directoryName);
          }
          // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer
          // the full size of the file, but does not waste memory.
          // The "using" will close the stream even if an exception occurs.
          using FileStream streamWriter = File.Create(fullZipToPath);
          StreamUtils.Copy(zipStream, streamWriter, buffer);
        }
      } finally {
        if (zipFile != null) {
          zipFile.IsStreamOwner = true; // Makes close also shut the underlying stream
          zipFile.Close(); // Ensure we release resources
        }
      }
    }

    [ExcludeFromCodeCoverage]
    protected virtual DateTime GetBackupDateTime() {
      return DateTime.Now;
    }

    /// <summary>
    ///   Zips all the files inside the specified folder (non-recursive) into a zip file.
    /// </summary>
    /// <remarks>
    ///   Based on
    ///   https://ourcodeworld.com/articles/read/629/how-to-create-and-extract-zip-files-compress-and-decompress-zip-with-sharpziplib-with-csharp-in-winforms.
    /// </remarks>
    private static void ZipFolder(string folderPath, string outputFilePath,
      int compressionLevel = 9) {
      // Depending on the directory this could be very large and would require more
      // attention in a commercial package.
      string[] filenames = Directory.GetFiles(folderPath);
      // 'using' statements guarantee the stream is closed properly which is a big
      // source of problems otherwise.  It's exception safe as well which is great.
      using ZipOutputStream outputStream =
        new ZipOutputStream(File.Create(outputFilePath));
      // Define the compression level
      // 0 - store only to 9 - means best compression
      outputStream.SetLevel(compressionLevel);
      byte[] buffer = new byte[4096];
      foreach (string file in filenames) {
        ZipEntry entry = new ZipEntry(Path.GetFileName(file)) {DateTime = DateTime.Now};
        // Setup the entry data as required.
        // Crc and size are handled by the library for seekable streams
        // so no need to do them here.
        // Could also use the last write time or similar for the file.
        outputStream.PutNextEntry(entry);
        using FileStream fs = File.OpenRead(file);
        // Using a fixed size buffer here makes no noticeable difference for output
        // but keeps a lid on memory usage.
        int sourceBytes;
        do {
          sourceBytes = fs.Read(buffer, 0, buffer.Length);
          outputStream.Write(buffer, 0, sourceBytes);
        } while (sourceBytes > 0);
      }
    }
  }
}