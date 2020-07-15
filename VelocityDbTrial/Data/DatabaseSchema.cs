using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class DatabaseSchema {

    public DatabaseSchema(int expectedVersionNo, SessionNoServer session) {
      ExpectedVersionNo = expectedVersionNo;
      Session = session;
    }

    //private readonly string _defaultDatabaseFolderPath =
    //  "E:\\Simon\\OneDrive\\Documents\\Software\\Sound Explorers Audio Archive\\Database";

    private int ExpectedVersionNo { get; }
    private bool Exists {
      get {
        return Session.ContainsDatabase(Session.DatabaseLocations.First(), 1);
      }
    }
    private SessionNoServer Session { get; }
    private SchemaVersion Version { get; set; }

    public bool IsUpToDate {
      get {
        try {
          if (Exists) {
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

    public void Update() {
      try {
        if (Exists) {
          Version = GetVersionIfExists();
        }
        Session.BeginUpdate();
        if (Version == null) {
          Session.RegisterClass(typeof(SchemaVersion));
          Version = new SchemaVersion();
        }
        Version.Number = ExpectedVersionNo;
        Session.Persist(Version);
        Session.RegisterClass(typeof(Event));
        Session.RegisterClass(typeof(Location));
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
