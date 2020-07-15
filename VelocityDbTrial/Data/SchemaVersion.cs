using System.Linq;
using VelocityDb;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class SchemaVersion : OptimizedPersistable {

    private int _number;

    public int Number {
      get {
        return _number;
      }
      set {
        Update();
        _number = value;
      }
    }

    public int ExpectedNumber { get; private set; }
    public bool IsUpToDate => Number == ExpectedNumber;

    private SchemaVersion() {
    }

    public static SchemaVersion Read(int expectedNumber, SessionNoServer session) {
      SchemaVersion version = null;
      try {
        if (SchemaExists(session)) {
          version = FindVersion(session);
        }
        if (version == null) {
          version = AddVersion(session);
        }
      } catch {
        session.Abort();
        throw;
      }
      version.ExpectedNumber = expectedNumber;
      return version;
    }

    public void Upgrade() {
      try {
        Session.BeginUpdate();
        Session.RegisterClass(typeof(Event));
        Session.RegisterClass(typeof(Location));
        Number = ExpectedNumber;
        Session.Commit();
      } catch {
        Session.Abort();
        throw;
      }
    }

    private static SchemaVersion AddVersion(SessionNoServer session) {
      session.BeginUpdate();
      session.RegisterClass(typeof(SchemaVersion));
      var version = new SchemaVersion();
      session.Persist(version);
      session.Commit();
      return version;
    }

    private static SchemaVersion FindVersion(SessionNoServer session) {
      session.BeginRead();
      SchemaVersion version = session.AllObjects<SchemaVersion>().FirstOrDefault();
      session.Commit();
      return version;
    }

    private static bool SchemaExists(SessionNoServer session) {
      return session.ContainsDatabase(session.DatabaseLocations.First(), dbNum: 1);
    }

  }
}
