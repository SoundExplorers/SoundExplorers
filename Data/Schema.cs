﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    private static Schema? _instance;
    private IEnumerable<Type>? _entityTypes;
    private IEnumerable<Type>? _persistableTypes;
    private IEnumerable<RelationInfo>? _relations;
    private IDictionary<Type, Type>? _rootTypes;

    // private IDictionary<Type, Type>? _rootTypes;
    private int _version;

    /// <summary>
    ///   The instance of the schema that is referenced by default by entities.
    /// </summary>
    public static Schema Instance {
      get => _instance ??= new Schema();
      set => _instance = value;
    }

    /// <summary>
    ///   From VelocityDB User's Guide:
    ///   'It is recommended that you make the following override in your
    ///   OptimizedPersistable subclass for better performance. ...
    ///   We may make this default but it could break existing code
    ///   so it is not a trivial change.'
    /// </summary>
    public override bool AllowOtherTypesOnSamePage => false;

    /// <summary>
    ///   Enumerates the entity types persisted on the database.
    /// </summary>
    public IEnumerable<Type> EntityTypes =>
      _entityTypes ??= CreateEntityTypes();

    /// <summary>
    ///   The structure of on-to-many relations between entity types.
    /// </summary>
    public IEnumerable<RelationInfo> Relations =>
      _relations ??= CreateRelations();

    public IDictionary<Type, Type> RootTypes =>
      _rootTypes ??= CreateRootTypes();

    /// <summary>
    ///   Gets or sets the schema version. Zero initially. Not the same as the
    ///   application version.
    /// </summary>
    public int Version {
      get => _version;
      set {
        UpdateNonIndexField();
        _version = value;
      }
    }

    /// <summary>
    ///   Enumerates the types persisted on the database.
    /// </summary>
    private IEnumerable<Type> PersistableTypes =>
      _persistableTypes ??= CreatePersistableTypes();

    /// <summary>
    ///   Returns the one Schema entity, if it already exists on the database,
    ///   otherwise null.
    /// </summary>
    public static Schema? Find(QueryHelper queryHelper,
      SessionBase session) {
      return queryHelper.FindSingleton<Schema>(session);
    }

    /// <summary>
    ///   Registers the persistable types on the database,
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
    public void RegisterPersistableTypes(SessionBase session) {
      foreach (var persistableType in PersistableTypes) {
        session.RegisterClass(persistableType);
      }
    }

    protected virtual IEnumerable<Type> CreatePersistableTypes() {
      var list = new List<Type> {
        typeof(Act),
        typeof(SortedEntityCollection<Act>),
        typeof(Artist),
        typeof(SortedEntityCollection<Artist>),
        typeof(Credit),
        typeof(SortedEntityCollection<Credit>),
        typeof(Event),
        typeof(SortedEntityCollection<Event>),
        typeof(EventType),
        typeof(SortedEntityCollection<EventType>),
        typeof(Genre),
        typeof(SortedEntityCollection<Genre>),
        typeof(Location),
        typeof(SortedEntityCollection<Location>),
        typeof(Newsletter),
        typeof(SortedEntityCollection<Newsletter>),
        typeof(Series),
        typeof(SortedEntityCollection<Series>),
        typeof(Piece),
        typeof(SortedEntityCollection<Piece>),
        typeof(Role),
        typeof(SortedEntityCollection<Role>),
        typeof(Schema),
        typeof(Set),
        typeof(SortedEntityCollection<Set>),
        typeof(UserOption),
        typeof(SortedEntityCollection<UserOption>)
      };
      return list.ToArray();
    }

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

    protected virtual IDictionary<Type, Type> CreateRootTypes() {
      return new Dictionary<Type, Type> {
        [typeof(Act)] = typeof(SortedEntityCollection<Act>),
        [typeof(Artist)] = typeof(SortedEntityCollection<Artist>),
        [typeof(Credit)] = typeof(SortedEntityCollection<Credit>),
        [typeof(Event)] = typeof(SortedEntityCollection<Event>),
        [typeof(EventType)] = typeof(SortedEntityCollection<EventType>),
        [typeof(Genre)] = typeof(SortedEntityCollection<Genre>),
        [typeof(Location)] = typeof(SortedEntityCollection<Location>),
        [typeof(Newsletter)] = typeof(SortedEntityCollection<Newsletter>),
        [typeof(Series)] = typeof(SortedEntityCollection<Series>),
        [typeof(Piece)] = typeof(SortedEntityCollection<Piece>),
        [typeof(Role)] = typeof(SortedEntityCollection<Role>),
        [typeof(Set)] = typeof(SortedEntityCollection<Set>),
        [typeof(UserOption)] = typeof(SortedEntityCollection<UserOption>)
      };
    }

    private IEnumerable<Type> CreateEntityTypes() {
      return (
        from persistableType in PersistableTypes
        where persistableType.GetInterfaces().Contains(typeof(IEntity))
        select persistableType).ToList();
    }
  }
}