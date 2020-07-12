using VelocityDb;

namespace SoundExplorersDatabase.Data {
  internal class SchemaVersion : OptimizedPersistable {
    private int _number;

    public override string ToString() { return Number.ToString(); }

    public int Number {

      get {
        return _number;
      }

      set {
        Update();
        _number = value;
      }
    }
  }
}
