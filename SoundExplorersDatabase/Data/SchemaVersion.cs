using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class SchemaVersion : OptimizedPersistable {
    private int _number;

    private SchemaVersion() { }

    public int Number {
      get => _number;
      set {
        Update();
        _number = value;
      }
    }

    public int ExpectedNumber { get; private set; }
    public bool IsUpToDate => Number == ExpectedNumber;

    public static SchemaVersion Read(int expectedNumber, SessionBase session) {
      SchemaVersion version = null;
      try {
        if (SchemaExists(session)) {
          version = Find(session);
        }
        if (version == null) {
          version = AddVersion(session);
        }
      }
      catch {
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
      }
      catch {
        Session.Abort();
        throw;
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

    private static bool SchemaExists([NotNull] SessionBase session) {
      return session.ContainsDatabase(session.DatabaseLocations.First(), 1);
    }
  }
}