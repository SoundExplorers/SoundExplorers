using System;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class DatabaseSchema : IDisposable {

    public DatabaseSchema(int expectedVersionNo, string databaseFolderPath = null) {
      ExpectedVersionNo = expectedVersionNo;
      if (databaseFolderPath == null) {
        databaseFolderPath = _defaultDatabaseFolderPath;
      }
      Session = new SessionNoServer(databaseFolderPath);
    }

    private readonly string _defaultDatabaseFolderPath =
      "E:\\Simon\\OneDrive\\Documents\\Software\\Sound Explorers Audio Archive\\Database";

    private int ExpectedVersionNo { get; }
    private bool SchemaExists {
      get {
        return Session.ContainsDatabase(Session.DatabaseLocations.First(), 1);
      }
    }
    private SessionNoServer Session { get; }
    private SchemaVersion Version { get; set; }

    public bool IsUpToDate {
      get {
        try {
          if (SchemaExists) {
            Version = GetVersionIfExists();
            if (Version == null) {
              return false;
            }
            if (Version.Number == ExpectedVersionNo) {
              return true;
            } else {
              return false;
            }
          } else { // Schema does not exist
            return false;
          }
        } catch {
          Session.Abort();
          throw;
        }
      }
    }

    public int VersionNo {
      get {
        if (Version != null) {
          return Version.Number;
        } else {
          return -1;
        }
      }
    }

    public void Dispose() {
      Session.Dispose();
    }

    public void Update() {
      try {
        if (SchemaExists) {
          Version = GetVersionIfExists();
        }
        Session.BeginUpdate();
        if (Version == null) {
          Session.RegisterClass(typeof(SchemaVersion));
          Version = new SchemaVersion();
        }
        Version.Number = ExpectedVersionNo;
        Session.Persist(Version);
        Session.Commit();
      } catch {
        Session.Abort();
        throw;
      }
    }

    private SchemaVersion GetVersionIfExists() {
      Session.BeginRead();
      SchemaVersion version = Session.AllObjects<SchemaVersion>().FirstOrDefault();
      Session.Commit();
      return version;
    }

  }
}
