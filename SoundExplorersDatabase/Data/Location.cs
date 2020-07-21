using System.Data;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Collection.BTree.Extensions;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Location : OptimizedPersistable {
    private LocationEvents _events;

    [Index] [VelocityDb.Indexing.UniqueConstraint]
    private string _name;

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

    public LocationEvents Events =>
      _events ?? (_events = new LocationEvents(this));

    [NotNull]
    public static Location Read([NotNull] string name,
      [NotNull] SessionBase session) {
      // ReSharper disable once ReplaceWithSingleCallToFirst
      return session.Index<Location>("_name")
        .Where(location => location.Name == name).First();
      // Seems not to use the direct index lookup instead of the default Enumerable.Where:
      // return (
      //   from Location location in session.Index<Location>("_name")
      //   where location.Name == name
      //   select location).First();
      //
      // Same with this.
      // return session
      //   .Index<Location>("_name")
      //   .First(location => location.Name == name);
    }

    public override void Unpersist(SessionBase session) {
      // I would expect VelocityDB to throw a ReferentialIntegrityException
      // if the parent had children.
      // But it does not, even in their Relations sample,
      // where Customer.Unpersist still works,
      // even when commenting out all code in it except for base.Unpersist .
      if (Events.Count == 0) {
        base.Unpersist(session);
      }
      else {
        throw new ConstraintException(
          $"Location '{Name}' cannot be deleted because it has {Events.Count} events.");
      }
    }
  }
}