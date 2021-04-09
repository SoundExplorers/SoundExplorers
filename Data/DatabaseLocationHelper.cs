using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  [ExcludeFromCodeCoverage]
  public static class DatabaseLocationHelper {
    private static SessionBase Session { get; set; } = null!;

    /// <summary>
    ///   Ensures that the database locations system database file knows that the all the
    ///   database files are in the current database folder, in case this is a copy of a
    ///   database created in another folder, which may have been on another computer.
    /// </summary>
    public static void Localise(string databaseFolderPath) {
      Debug.WriteLine("DatabaseLocationHelper.Localise");
      Session = new SessionNoServer(databaseFolderPath);
      var defaultDatabaseLocation = Session.DefaultDatabaseLocation();
      if (!IsDatabaseLocationLocal(defaultDatabaseLocation)) {
        Debug.WriteLine("    Localising default database location");
        Session.RelocateDefaultDatabaseLocation();
      }
      Session.BeginUpdate();
      foreach (var database in Session.Databases) {
        if (!IsDatabaseLocationLocal(database.Location)) {
          Debug.WriteLine($"    Localising database location {database.Location}");
          Session.RelocateDatabaseLocationFor(
            database.DatabaseNumber, SessionBase.LocalHost, 
            Session.SystemDirectory);
        }
      }
      Session.Commit();
    }

    private static bool IsDatabaseLocationLocal(
      DatabaseLocation databaseLocation) {
      return string.Compare(
               databaseLocation.HostName, SessionBase.LocalHost,
               StringComparison.Ordinal) == 0 && 
             string.Compare(
               databaseLocation.DirectoryPath, Session.SystemDirectory,
               StringComparison.Ordinal) == 0;
    }
  }
}