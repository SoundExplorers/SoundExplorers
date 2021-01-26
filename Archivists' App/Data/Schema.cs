using System;
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
    private IEnumerable<Type> _entityTypes;
    private IEnumerable<RelationInfo> _relations;
    private int _version;

    /// <summary>
    ///   The instance of the schema that is referenced by default by entities.
    /// </summary>
    [NotNull]
    public static Schema Instance {
      get => _instance ??= new Schema();
      set => _instance = value;
    }

    /// <summary>
    ///   Enumerates the entity types persisted on the database
    /// </summary>
    [NotNull]
    private IEnumerable<Type> EntityTypes =>
      _entityTypes ??= CreateEntityTypes();

    /// <summary>
    ///   The structure of on-to-many relations between entity types.
    /// </summary>
    [NotNull]
    public IEnumerable<RelationInfo> Relations =>
      _relations ??= CreateRelations();

    /// <summary>
    ///   Gets or sets the schema version.
    ///   Zero initially.
    ///   Not the same as the application version.
    /// </summary>
    public int Version {
      get => _version;
      set {
        UpdateNonIndexField();
        _version = value;
      }
    }

    /// <summary>
    ///   From VelocityDB User's Guide:
    ///   'It is recommended that you make the following override in your
    ///   OptimizedPersistable subclass for better performance. ...
    ///   We may make this default but it could break existing code
    ///   so it is not a trivial change.'
    /// </summary>
    public override bool AllowOtherTypesOnSamePage => false;

    private static IEnumerable<Type> CreateEntityTypes() {
      var list = new List<Type> {
        typeof(Act),
        typeof(Artist),
        typeof(Credit),
        typeof(Event),
        typeof(EventType),
        typeof(Genre),
        typeof(Location),
        typeof(Newsletter),
        typeof(Series),
        typeof(Piece),
        typeof(Role),
        typeof(Schema),
        typeof(Set),
        typeof(UserOption)
      };
      return list.ToArray();
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
    ///   Registers the entity types on the database,
    ///   so that a VelocityDB licence file ('license database')
    ///   will not have to be included in the user's database.
    /// </summary>
    /// <param name="session">Database session</param>
    /// <remarks>
    ///   How this will work, according to VelocityDB User Guide:
    ///   Application Deployment and VelocityDB license check
    ///   Normally you need to deploy the license database, 4.odb, but if you are publishing your
    ///   application as open source or your database files in a publicly accessible directory then do not
    ///   include 4.odb since that would enable unlicensed usage of VelocityDB. Instead register all your
    ///   persistent classes prior to deployment and deploy database 1.odb which then contains your entire
    ///   database schema. VelocityDB may do a license check whenever database schema is added to or is
    ///   updated.
    /// </remarks>
    public void RegisterEntityTypes([NotNull] SessionBase session) {
      foreach (var entityType in EntityTypes) {
        session.RegisterClass(entityType);
      }
    }
  }
}