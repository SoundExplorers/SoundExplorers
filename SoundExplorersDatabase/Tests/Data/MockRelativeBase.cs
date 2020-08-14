using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  public abstract class MockRelativeBase : IRelative {
    private RelativeBase _identifyingParent;
    private string _simpleKey;

    protected MockRelativeBase() {
      Key = new Key(SimpleKey, IdentifyingParent);
    }

    public RelativeBase IdentifyingParent {
      get => _identifyingParent;
      protected set {
        _identifyingParent = value;
        Key = new Key(SimpleKey, value);
      }
    }

    public Key Key { get; private set; }

    public string SimpleKey {
      get => _simpleKey;
      protected set {
        _simpleKey = value;
        Key = new Key(value, IdentifyingParent);
      }
    }
  }
}