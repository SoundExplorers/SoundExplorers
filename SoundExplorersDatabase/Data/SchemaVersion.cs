using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class SchemaVersion : OptimizedPersistable {
    private int _number;

    private SchemaVersion() { }

    private int ExpectedNumber { get; set; }
    public bool IsUpToDate => Number == ExpectedNumber;

    public int Number {
      get => _number;
      set {
        Update();
        _number = value;
      }
    }

    private static SchemaVersion AddVersion(SessionBase session) {
      session.BeginUpdate();
      session.RegisterClass(typeof(SchemaVersion));
      var version = new SchemaVersion();
      session.Persist(version);
      session.Commit();
      return version;
    }

    [CanBeNull]
    private static SchemaVersion Find([NotNull] SessionBase session) {
      session.BeginRead();
      var version = session.AllObjects<SchemaVersion>().FirstOrDefault();
      session.Commit();
      return version;
    }

    public static SchemaVersion Read(int expectedNumber, SessionBase session) {
      SchemaVersion version = null;
      try {
        // TODO Remove dependence on QueryHelper.Instance for multi-thread testing. 
        if (QueryHelper.Instance.SchemaExistsOnDatabase(session)) {
          version = Find(session);
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
  }
}