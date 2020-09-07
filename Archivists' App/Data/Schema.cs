using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Database schema entity.
  /// </summary>
  /// <remarks>
  ///   This is a single occurence entity with no key, parents or children.
  ///   So it inherits from OptimizedPersistable instead of EntityBase.
  /// </remarks>
  public class Schema : OptimizedPersistable {
    private static Schema _instance;
    private IEnumerable<RelationInfo> _relations;
    private int _version;

    /// <summary>
    ///   The instance of the schema that is referenced by default by entities.
    /// </summary>
    [NotNull]
    public static Schema Instance => _instance ?? (_instance = new Schema());

    /// <summary>
    ///   The structure of on-to-many relations between entity types.
    /// </summary>
    [NotNull]
    public IEnumerable<RelationInfo> Relations =>
      _relations ?? (_relations = CreateRelations());

    /// <summary>
    ///   The schema version.
    ///   Not the same as the application version.
    /// </summary>
    public int Version {
      get => _version;
      private set {
        UpdateNonIndexField();
        _version = value;
      }
    }

    [NotNull]
    protected virtual IEnumerable<RelationInfo> CreateRelations() {
      var list = new List<RelationInfo> {
        new RelationInfo(typeof(Act), typeof(Set), false),
        new RelationInfo(typeof(Artist), typeof(Credit), true),
        new RelationInfo(typeof(Event), typeof(Set), true),
        new RelationInfo(typeof(EventType), typeof(Event), true),
        new RelationInfo(typeof(Genre), typeof(Set), true),
        new RelationInfo(typeof(Location), typeof(Event), true),
        new RelationInfo(typeof(Newsletter), typeof(Event), false),
        new RelationInfo(typeof(Series), typeof(Event), false),
        new RelationInfo(typeof(Piece), typeof(Credit), true),
        new RelationInfo(typeof(Role), typeof(Credit), true),
        new RelationInfo(typeof(Set), typeof(Piece), true)
      };
      return list.ToArray();
    }

    /// <summary>
    ///   Returns the one Schema entity, if it already exists on the database,
    ///   otherwise null.
    /// </summary>
    [CanBeNull]
    public static Schema Find([NotNull] QueryHelper queryHelper,
      [NotNull] SessionBase session) {
      return queryHelper.SchemaExistsOnDatabase(session)
        ? session.AllObjects<Schema>().FirstOrDefault()
        : null;
    }

    /// <summary>
    ///   Upgrades the schema on the database.
    ///   The entity types are registered so that a VelocityDB licence file
    ///   ('license database') will not have to be included in the database.
    /// </summary>
    /// <param name="newVersion">New schema version number</param>
    /// <param name="session">Database session</param>
    public void Upgrade(int newVersion, [NotNull] SessionBase session) {
      session.RegisterClass(typeof(Act));
      session.RegisterClass(typeof(Artist));
      session.RegisterClass(typeof(Credit));
      session.RegisterClass(typeof(Event));
      session.RegisterClass(typeof(EventType));
      session.RegisterClass(typeof(Genre));
      session.RegisterClass(typeof(Location));
      session.RegisterClass(typeof(Newsletter));
      session.RegisterClass(typeof(Piece));
      session.RegisterClass(typeof(Role));
      session.RegisterClass(typeof(Schema));
      session.RegisterClass(typeof(Series));
      session.RegisterClass(typeof(Set));
      session.RegisterClass(typeof(UserOption));
      Version = newVersion;
    }
  }
}