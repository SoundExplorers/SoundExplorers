using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Collection.BTree.Extensions;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Location : ReferenceTracked {
    private LocationEvents _events;

    [Index] [UniqueConstraint] private string _name;

    private string _notes;

    public Location([NotNull] string name) {
      Name = name;
    }

    [FieldAccessor("_name")]
    [PublicAPI]
    public string Name {
      get => _name;
      set {
        Update();
        _name = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        Update();
        _notes = value;
      }
    }

    public LocationEvents Events {
      get {
        if (_events != null) {
          return _events;
        }
        Update();
        _events = new LocationEvents(this);
        return _events;
      }
    }

    [NotNull]
    public static Location Read([NotNull] string name,
      [NotNull] SessionBase session) {
      // ReSharper disable once ReplaceWithSingleCallToFirst
      return session.Index<Location>("_name")
        .Where(location => location.Name == name).First();
      // Seems not to use the direct index lookup instead of the default Enumerable.Where
      //
      // return (
      //   from Location location in session.Index<Location>("_name")
      //   where location.Name == name
      //   select location).First();
      //
      // return session
      //   .Index<Location>("_name")
      //   .First(location => location.Name == name);
    }
  }
}