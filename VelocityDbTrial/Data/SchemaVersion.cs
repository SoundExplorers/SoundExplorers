using VelocityDb;

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

  }
}
