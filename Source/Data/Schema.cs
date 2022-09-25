using System;
using System.Collections.Generic;
using VelocityDb;
using VelocityDb.Collection;
using VelocityDb.Collection.BTree;
using VelocityDb.Collection.Comparer;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorers.Data; 

/// <summary>
///   Database schema entity.
/// </summary>
/// <remarks>
///   This is a single occurence entity with no key, parents or children.
///   So it inherits from OptimizedPersistable instead of EntityBase.
/// </remarks>
public class Schema : OptimizedPersistable {
  private static Schema? _instance;

  /// <summary>
  ///   Fields that are only working variables are marked with
  ///   <see cref="NonSerializedAttribute" /> to stop them being persisted.
  /// </summary>
  [NonSerialized] private IEnumerable<Type>? _persistableTypes;

  [NonSerialized] private IEnumerable<RelationInfo>? _relations;
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
  ///   The structure of on-to-many relations between entity types.
  /// </summary>
  public IEnumerable<RelationInfo> Relations =>
    _relations ??= CreateRelations();

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
  ///   <para>
  ///     See also VelocityDB User Guide section 'Register all types that you plan on
  ///     persisting', especially the subsection 'If your application schema is using
  ///     indexes'.
  ///   </para>
  /// </remarks>
  public void RegisterPersistableTypes(SessionBase session) {
    foreach (var persistableType in PersistableTypes) {
      session.RegisterClass(persistableType);
    }
    // Register additional index-related classes.
    session.RegisterClass(typeof(IndexDescriptor));
    session.RegisterClass(typeof(BTreeSetOidShort<IndexDescriptor>));
    session.RegisterClass(typeof(CompareByField<IndexDescriptor>));
    session.RegisterClass(typeof(Indexes)); 
    session.RegisterClass(typeof(VelocityDbList<OptimizedPersistable>));
    // Not mentioned in manual, but definitely required, presumably because EntityBase
    // accesses References.
    session.RegisterClass(typeof(Reference));
    session.RegisterClass(typeof(BTreeSet<Reference>));
  }

  /// <summary>
  ///   Creates the list of persistable types that need to be registered.
  /// </summary>
  /// <remarks>
  ///   For each persistable type that uses indexes, currently all entity types (types
  ///   that derive from EntityBase), the corresponding BTreeSet and
  ///   CompareByFieldIndex need to be registered too. CompareByFieldIndex registration
  ///   is not strictly necessary, at this stage, as the application does not use
  ///   CompareByField or CompareByFieldIndex. But that could change! Custom entity
  ///   property types (and their custom property types recursively) need to be
  ///   registered too.
  /// </remarks>
  protected virtual IEnumerable<Type> CreatePersistableTypes() {
    var list = new List<Type> {
      // Custom entity property types. 
      typeof(Key),
      typeof(SortedEntityCollection<Credit>),
      typeof(SortedEntityCollection<Event>),
      typeof(SortedEntityCollection<Piece>),
      typeof(SortedEntityCollection<Set>),
      // Recursive property type. SortedEntityCollection.Comparer is set to it.
      typeof(KeyComparer),  
      // Entity types, with corresponding BTreeSets and CompareByFieldIndexes if the
      // entity type uses indexes.
      typeof(Act),
      typeof(BTreeSet<Act>),
      typeof(CompareByFieldIndex<Act>),
      typeof(Artist),
      typeof(BTreeSet<Artist>),
      typeof(CompareByFieldIndex<Artist>),
      typeof(Credit),
      typeof(BTreeSet<Credit>),
      typeof(CompareByFieldIndex<Credit>),
      typeof(Event),
      typeof(BTreeSet<Event>),
      typeof(CompareByFieldIndex<Event>),
      typeof(EventType),
      typeof(BTreeSet<EventType>),
      typeof(CompareByFieldIndex<EventType>),
      typeof(Genre),
      typeof(BTreeSet<Genre>),
      typeof(CompareByFieldIndex<Genre>),
      typeof(Location),
      typeof(BTreeSet<Location>),
      typeof(CompareByFieldIndex<Location>),
      typeof(Newsletter),
      typeof(BTreeSet<Newsletter>),
      typeof(CompareByFieldIndex<Newsletter>),
      typeof(Series),
      typeof(BTreeSet<Series>),
      typeof(CompareByFieldIndex<Series>),
      typeof(Piece),
      typeof(BTreeSet<Piece>),
      typeof(CompareByFieldIndex<Piece>),
      typeof(Role),
      typeof(BTreeSet<Role>),
      typeof(CompareByFieldIndex<Role>),
      // Simple registration for the one persistable type that does not use indexes.
      typeof(Schema),
      typeof(Set),
      typeof(BTreeSet<Set>),
      typeof(CompareByFieldIndex<Set>),
      typeof(UserOption),
      typeof(BTreeSet<UserOption>),
      typeof(CompareByFieldIndex<UserOption>)
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
}