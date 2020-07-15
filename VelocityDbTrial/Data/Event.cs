using VelocityDb;
using VelocityDb.Indexing;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Event : ReferenceTracked {

    public Event(Location location) {
      location.AddEvent(this);
    }

    [Index]
    [UniqueConstraint]
    private string _name;

    private string _notes;

    [FieldAccessor("_name")]
    public string Name {
      get {
        return _name;
      }
      set {
        Update();
        _name = value;
      }
    }

    public string Notes {
      get {
        return _notes;
      }
      set {
        Update();
        _notes = value;
      }
    }

  }
}
