using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VelocityDb.Session;

namespace SoundExplorers.Data; 

[ExcludeFromCodeCoverage]
public static class DatabaseLocationHelper {
  private static SessionBase Session { get; set; } = null!;

  /// <summary>
  ///   Ensures that the database locations system database file knows that the all the
  ///   database files are in the current database folder, in case this is a copy of a
  ///   database created in another folder, which may have been on another computer.
  /// </summary>
  /// <remarks>
  ///   In an email message, Mats at VelocityDB wrote
  ///   "SessionBase.RelocateDefaultDatabaseLocation is done automatically in an update
  ///   transaction in the latest release, see https://velocitydb.com/ReleaseNotes.pdf.
  ///   It doesn't hurt to do it manually as you do but I had some request to do it
  ///   behind the covers."
  ///   <para>
  ///     I cannot find the reference in the release notes. And
  ///     I got the problem again when I tried to get the sample database working on my
  ///     new computer. I found that the check to see whether the database files are
  ///     already local (the now commented out IsDatabaseLocationLocal method) no longer
  ///     returned false when the localisation needed doing.  So, for now, the
  ///     localisation is done unconditionally.  That's fine, as it is too fast to be
  ///     noticeable.
  ///   </para>
  /// </remarks>
  public static void Localise(string databaseFolderPath) {
    // Debug.WriteLine("DatabaseLocationHelper.Localise");
    Session = new SessionNoServer(databaseFolderPath);
    // Checking whether the database file is already local does not work.
    // So do the localisation unconditionally.
    //var defaultDatabaseLocation = Session.DefaultDatabaseLocation();
    // Debug.WriteLine("    Localising default database location");
    Session.RelocateDefaultDatabaseLocation();
    // if (!IsDatabaseLocationLocal(defaultDatabaseLocation)) {
    //   Debug.WriteLine("    Localising default database location");
    //   Session.RelocateDefaultDatabaseLocation();
    // }
    Session.BeginUpdate();
    foreach (var database in Session.Databases) {
      // Checking whether the database file is already local does not work.
      // So do the localisation unconditionally.
      Debug.WriteLine($"    Localising database location {database.Location}");
      Session.RelocateDatabaseLocationFor(
        database.DatabaseNumber, SessionBase.LocalHost,
        Session.SystemDirectory);
      // if (!IsDatabaseLocationLocal(database.Location)) {
      //   Debug.WriteLine($"    Localising database location {database.Location}");
      //   Session.RelocateDatabaseLocationFor(
      //     database.DatabaseNumber, SessionBase.LocalHost, 
      //     Session.SystemDirectory);
      // }
    }

    Session.Commit();
  }

  // private static bool IsDatabaseLocationLocal(
  //   DatabaseLocation databaseLocation) {
  //   return string.Compare(
  //            databaseLocation.HostName, SessionBase.LocalHost,
  //            StringComparison.Ordinal) == 0 && 
  //          string.Compare(
  //            databaseLocation.DirectoryPath, Session.SystemDirectory,
  //            StringComparison.Ordinal) == 0;
  // }
}