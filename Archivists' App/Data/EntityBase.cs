using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using VelocityDb;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Base class for entity types that have one-to-many
  ///   and/or many-to-one relations with other entity types.
  /// </summary>
  public abstract class EntityBase : ReferenceTracked, IEntity {
    private IDictionary<Type, IDictionary>? _childrenOfType;
    private IDictionary<Type, IRelationInfo>? _childrenRelations;
    private EntityBase? _identifyingParent;
    private IDictionary<Type, IRelationInfo>? _parentRelations;
    private IDictionary<Type, EntityBase?>? _parents;
    private QueryHelper? _queryHelper;
    private Schema? _schema;
    private string _simpleKey = null!;

    static EntityBase() {
      DefaultDate = DateTime.Parse("1900/01/01");
    }

    /// <summary>
    ///   Creates an instance of an entity.
    /// </summary>
    /// <param name="entityType">
    ///   The main Type as which the entity will be persisted on the database.
    /// </param>
    /// <param name="simpleKeyName">
    ///   The name, for use in error messages, of the perstistable
    ///   public property corresponding to the simple key.
    /// </param>
    /// <param name="identifyingParentType">
    ///   Where applicable, the entity type of the identifying parent entity.
    /// </param>
    protected EntityBase(Type entityType,
      string simpleKeyName, Type? identifyingParentType) {
      EntityType = entityType ??
                   throw new ArgumentNullException(
                     nameof(entityType));
      SimpleKeyName = simpleKeyName ??
                      throw new ArgumentNullException(nameof(simpleKeyName));
      IdentifyingParentType = identifyingParentType;
      Key = new Key(this);
    }

    protected bool AllowBlankSimpleKey { get; set; }

    private IDictionary<Type, IDictionary> ChildrenOfType {
      get {
        InitialiseIfNull(_childrenOfType);
        return _childrenOfType!;
      }
      set {
        UpdateNonIndexField();
        _childrenOfType = value;
      }
    }

    private IDictionary<Type, IRelationInfo> ChildrenRelations {
      get {
        InitialiseIfNull(_childrenRelations);
        return _childrenRelations!;
      }
      set {
        UpdateNonIndexField();
        _childrenRelations = value;
      }
    }

    /// <summary>
    ///   A hopefully safely old date, suitable for initialising Date fields
    ///   because it is compatible with calendar controls.
    /// </summary>
    public static DateTime DefaultDate { get; }

    /// <summary>
    ///   The main Type as which the entity will be persisted on the database.
    ///   Entities of subtypes may be persisted but will be members of the same
    ///   child collections, if any, as entities of the main Type.
    /// </summary>
    internal Type EntityType { get; }

    private Type? IdentifyingParentType { get; }
    private bool IsAddingToOrRemovingFromIdentifyingParent { get; set; }
    private bool IsTopLevel => Parents.Count == 0;

    private IDictionary<Type, IRelationInfo> ParentRelations {
      get {
        InitialiseIfNull(_parentRelations);
        return _parentRelations!;
      }
      set {
        UpdateNonIndexField();
        _parentRelations = value;
      }
    }

    private IDictionary<Type, EntityBase?> Parents {
      get {
        InitialiseIfNull(_parents);
        return _parents!;
      }
      set {
        UpdateNonIndexField();
        _parents = value;
      }
    }

    internal QueryHelper QueryHelper {
      get => _queryHelper ??= QueryHelper.Instance;
      set => _queryHelper = value;
    }

    protected Schema Schema {
      get => _schema ??= Schema.Instance;
      set => _schema = value;
    }

    private string SimpleKeyName { get; }

    /// <summary>
    ///   From VelocityDB User's Guide:
    ///   'It is recommended that you make the following override in your
    ///   OptimizedPersistable subclass for better performance. ...
    ///   We may make this default but it could break existing code
    ///   so it is not a trivial change.'
    /// </summary>
    public override bool AllowOtherTypesOnSamePage => false;

    /// <summary>
    ///   The identifying parent entity, which, where applicable,
    ///   uniquely identifies this entity in combination with SimpleKey.
    /// </summary>
    /// <remarks>
    ///   Not applicable to top-level entity types,
    ///   i.e. those with no many-to-one  relations to other entity types.
    ///   Derived classes should set this to the value of the corresponding
    ///   perstistable public property.
    ///   This is null initially and must be set before persistence
    ///   to a parent entity, of type specified by IdentifyingParentType,
    ///   that already exists.
    /// </remarks>
    public EntityBase? IdentifyingParent {
      get => _identifyingParent;
      protected set {
        if (IsAddingToOrRemovingFromIdentifyingParent) {
          _identifyingParent = value;
          return;
        }
        if (IdentifyingParentType == null) {
          throw new ConstraintException(
            "An identifying parent type has not been specified " +
            $"for {EntityType.Name} '{Key}'.");
        }
        if (value == null) {
          throw new PropertyConstraintException(
            "A null reference has been specified as the " +
            $"{IdentifyingParentType.Name} for {EntityType.Name} '{Key}'.",
            IdentifyingParentType!.Name);
        }
        if (value.EntityType != IdentifyingParentType) {
          throw new PropertyConstraintException(
            $"A {value.EntityType.Name} has been specified as the " +
            $"IdentifyingParent for {EntityType.Name} '{Key}'. " +
            $"A {IdentifyingParentType.Name} is expected'.", IdentifyingParentType!.Name);
        }
        var newKey = new Key(SimpleKey, value);
        value.CheckForDuplicateChild(this, newKey);
        if (_identifyingParent != null &&
            // Should always be true
            _identifyingParent.ChildrenOfType[EntityType].Contains(Key)) {
          _identifyingParent.RemoveChild(this);
        }
        value.AddChild(this);
        Parents[IdentifyingParentType!] = value;
        _identifyingParent = value;
      }
    }

    /// <summary>
    ///   Derived from SimpleKey and, where applicable, IdentifyingParent,
    ///   this is used as the key of this entity in any
    ///   SortedChildLists of which it is a member.
    /// </summary>
    public Key Key { get; }

    /// <summary>
    ///   In combination with the optional IdentifyingParent,
    ///   uniquely identifies the entity.
    /// </summary>
    /// <remarks>
    ///   Derived classes should set this to the value,
    ///   converted to string if necessary, of the corresponding
    ///   perstistable public property.
    ///   It is null only initially.  It must be set before persistence.
    /// </remarks>
    public string SimpleKey {
      get => _simpleKey;
      protected set {
        CheckCanChangeSimpleKey(_simpleKey, value);
        _simpleKey = value;
      }
    }

    private void AddChild(EntityBase child) {
      UpdateNonIndexField();
      ChildrenOfType[child.EntityType].Add(CreateChildKey(child), child);
      // Full referential integrity is implemented in this class.
      // But, for added safety, update VelocityDB's internal referential integrity data. 
      References.AddFast(new Reference(child, "_children"));
    }

    internal void AddNonIdentifiedChild(EntityBase child) {
      CheckCanAddNonIdentifiedChild(child);
      AddChild(child);
      UpdateChild(child, this);
    }

    protected void ChangeNonIdentifyingParent(Type parentEntityType,
      EntityBase? newParent) {
      Parents[parentEntityType]?.RemoveChildWhenNonIdentifyingParentOrUnpersistingChild(
        this, newParent != null);
      newParent?.AddNonIdentifiedChild(this);
    }

    private void CheckCanAddNonIdentifiedChild(EntityBase child) {
      if (child == null) {
        throw new ConstraintException(
          "A null reference has been specified. " +
          $"So addition to {EntityType.Name} '{Key}' is not supported.");
      }
      CheckForDuplicateChild(child, CreateChildKey(child));
    }

    private void CheckCanChangeSimpleKey(
      string? oldSimpleKey, string? newSimpleKey) {
      if (!AllowBlankSimpleKey && string.IsNullOrWhiteSpace(newSimpleKey)) {
        throw new PropertyConstraintException(
          $"The {SimpleKeyName} is blank. " +
          $"Blank {SimpleKeyName}s are not supported.", SimpleKeyName);
      }
      if (newSimpleKey == oldSimpleKey) {
        return;
      }
      if (!IsTopLevel) {
        IdentifyingParent?.CheckForDuplicateChild(this,
          new Key(newSimpleKey, IdentifyingParent));
        return;
      }
      if (!IsPersistent || Session == null) {
        return;
      }
      // If there's no session, which means we cannot check for a duplicate,
      // EntityBase.UpdateNonIndexField should already have thrown 
      // an InvalidOperationException.
      if (QueryHelper.FindDuplicateSimpleKey(EntityType, Oid, newSimpleKey,
        Session) != null) {
        throw new PropertyConstraintException(
          $"The {EntityType.Name}'s {SimpleKeyName} cannot be set to " +
          $"'{newSimpleKey}' because another {EntityType.Name} " +
          $"with that {SimpleKeyName} already exists.", SimpleKeyName);
      }
    }

    protected virtual void CheckCanPersist(SessionBase session) {
      if (!AllowBlankSimpleKey && string.IsNullOrWhiteSpace(SimpleKey)) {
        throw new PropertyConstraintException(
          $"A {SimpleKeyName} has not yet been specified. " +
          $"So the {EntityType.Name} cannot be added.", SimpleKeyName);
      }
      foreach (var (parentType, parent) in Parents) {
        if (parent == null && ParentRelations[parentType].IsMandatory) {
          throw CreateParentNotSpecifiedException(
            EntityType.Name, Key, parentType.Name);
        }
      }
      if (IsTopLevel &&
          QueryHelper.FindDuplicateSimpleKey(EntityType, Oid, SimpleKey,
            session) != null) {
        throw new PropertyConstraintException(
          $"{EntityType.Name} '{Key}' " +
          $"cannot be added because another {EntityType.Name} "
          + "with the same key already exists.", SimpleKeyName);
      }
    }

    private void CheckCanRemoveChild(
      EntityBase child, bool isReplacingOrUnpersisting) {
      if (ChildrenRelations[child.EntityType].IsMandatory &&
          !isReplacingOrUnpersisting) {
        throw new ConstraintException(
          $"{child.EntityType.Name} '{child.Key}' " +
          $"cannot be removed from {EntityType.Name} '{Key}', " +
          "because membership is mandatory.");
      }
    }

    private void CheckForDuplicateChild(EntityBase child,
      Key keyToCheck) {
      var childrenOfType = ChildrenOfType[child.EntityType]!;
      bool childExists = childrenOfType.Contains(keyToCheck);
      if (childExists) {
        var existingChild = (EntityBase)childrenOfType[keyToCheck]!;
        if (!existingChild.Oid.Equals(child.Oid)) {
          throw new ConstraintException(
            $"{child.EntityType.Name} '{keyToCheck}' " +
            $"cannot be added to {EntityType.Name} '{Key}', " +
            $"because a {child.EntityType.Name} with that Key " +
            $"already belongs to the {EntityType.Name}.");
        }
      }
    }

    private Key CreateChildKey(EntityBase child) {
      return new Key(child,
        child.IdentifyingParentType == EntityType ? this : null);
    }

    private IDictionary<Type, IRelationInfo> CreateChildrenRelations() {
      var values =
        from relation in Schema.Relations
        where relation.ParentType == EntityType
        select relation;
      return values.ToDictionary<RelationInfo, Type, IRelationInfo>(
        value => value.ChildType, value => value);
    }

    public static PropertyConstraintException CreateParentNotSpecifiedException(
      string entityTypeName, Key key, string parentTypeName) {
      return new PropertyConstraintException(
        $"{entityTypeName} '{key}' " +
        $"cannot be added because its {parentTypeName} "
        + "has not been specified.", parentTypeName);
    }

    private IDictionary<Type, IRelationInfo> CreateParentRelations() {
      var values =
        from relation in Schema.Relations
        where relation.ChildType == EntityType
        select relation;
      return values.ToDictionary<RelationInfo, Type, IRelationInfo>(
        value => value.ParentType, value => value);
    }

    private string CreateReferentialIntegrityViolationMessage() {
      var list = new SortedList<string, int>();
      foreach (var childOfTypePair in ChildrenOfType) {
        if (childOfTypePair.Value.Count > 0) {
          list.Add(childOfTypePair.Key.Name, childOfTypePair.Value.Count);
        }
      }
      var writer = new StringWriter();
      writer.Write(
        $"{EntityType.Name} '{Key}' cannot be deleted because it is referenced by ");
      for (int i = 0; i < list.Count; i++) {
        writer.Write($"{list.Values[i]:N0} {list.Keys[i]}s");
        writer.Write(i < list.Count - 1 ? ", " : ".");
      }
      return writer.ToString();
    }

    public static string DateToSimpleKey(DateTime date) {
      return $"{date:yyyy/MM/dd}";
    }

    /// <summary>
    ///   Allows a derived entity to return
    ///   its SortedChildList of child entities of the specified entity type.
    /// </summary>
    protected abstract IDictionary GetChildren(Type childType);

    public static string GetIntegerSimpleKeyErrorMessage(string propertyName) {
      return $"{propertyName} must be an integer between 1 and 99.";
    }

    public static string IntegerToSimpleKey(int integer, string propertyName) {
      if (integer >= 1 && integer <= 99) {
        return integer.ToString().PadLeft(2, '0');
      }
      throw new PropertyValueOutOfRangeException(
        GetIntegerSimpleKeyErrorMessage(propertyName), propertyName);
    }

    private void Initialise() {
      ParentRelations = CreateParentRelations();
      Parents = new Dictionary<Type, EntityBase?>();
      foreach (var relationPair in ParentRelations) {
        Parents.Add(relationPair.Key, null);
      }
      ChildrenRelations = CreateChildrenRelations();
      ChildrenOfType = new Dictionary<Type, IDictionary>();
      foreach (var relationPair in ChildrenRelations) {
        ChildrenOfType.Add(relationPair.Key, GetChildren(relationPair.Key));
      }
    }

    private void InitialiseIfNull(object? backingField) {
      if (backingField == null) {
        Initialise();
      }
    }

    public override ulong Persist(Placement place, SessionBase session,
      bool persistRefs = true,
      bool disableFlush = false,
      Queue<IOptimizedPersistable>? toPersist = null) {
      // Debug.WriteLine($"EntityBase.Persist {EntityType.Name}");
      CheckCanPersist(session);
      return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }

    private void RemoveChild(EntityBase child) {
      // Debug.WriteLine($"EntityBase.RemoveChild {EntityType.Name}: removing {child.EntityType.Name} '{child.Key}'");
      UpdateNonIndexField();
      ChildrenOfType[child.EntityType].Remove(child.Key);
      // Full referential integrity is implemented in this class.
      // But, for added safety, update VelocityDB's internal referential integrity data. 
      References.Remove(References.First(r => r.To.Equals(child)));
    }

    private void RemoveChildWhenNonIdentifyingParentOrUnpersistingChild(
      EntityBase child, bool isReplacingOrUnpersisting) {
      CheckCanRemoveChild(child, isReplacingOrUnpersisting);
      RemoveChild(child);
      UpdateChild(child, null);
    }

    /// <summary>
    ///   Removes the entity from all its parent entities (if any) in preparation for its
    ///   deletion from the database.
    /// </summary>
    private void RemoveFromAllParents() {
      // Debug.WriteLine($"EntityBase.RemoveFromAllParents {EntityType.Name}");
      var nonIdentifyingParents = (
        from parent in Parents.Values 
        where parent != null && !parent.Equals(IdentifyingParent) 
        select parent).ToList();
      if (nonIdentifyingParents.Count > 0) {
        foreach (var nonIdentifyingParent in nonIdentifyingParents) {
          nonIdentifyingParent!.RemoveChildWhenNonIdentifyingParentOrUnpersistingChild(
            this, true);
        }
      }
      if (IdentifyingParent != null) {
        // Removing the entity from its identifying parent last to prevent
        // VelocityDB FlushUpdates error that can happen on Unpersist.
        IdentifyingParent!.RemoveChildWhenNonIdentifyingParentOrUnpersistingChild(
          this, true);
      }
    }

    /// <summary>
    ///   Allows a derived entity to update the field (not property)
    ///   corresponding to the parent entity of the specified entity type
    ///   with the specified new value.
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected virtual void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase? newParent) {
      throw new NotSupportedException();
    }

    public override void Unpersist(SessionBase session) {
      // Debug.WriteLine($"EntityBase.Unpersist {EntityType.Name}");
      if (References.Count > 0) {
        // If we did not do this, VelocityDB would throw a ReferentialIntegrityException
        // on base.Unpersist.
        throw new ConstraintException(
          CreateReferentialIntegrityViolationMessage());
      }
      RemoveFromAllParents();
      base.Unpersist(session);
    }

    private void UpdateChild(EntityBase child,
      EntityBase? newParent) {
      child.UpdateNonIndexField();
      child.Parents[EntityType] = newParent;
      if (EntityType == child.IdentifyingParentType) {
        child.IsAddingToOrRemovingFromIdentifyingParent = true;
        child.IdentifyingParent = newParent;
        child.IsAddingToOrRemovingFromIdentifyingParent = false;
      } else {
        child.SetNonIdentifyingParentField(EntityType,
          newParent);
      }
    }

    /// <summary>
    ///   Marks the entity as being updated, so that the entity will be written
    ///   at commit transaction. In this application, this is used instead of
    ///   OptimizedPersistable.Update, even for key properties,
    ///   because we don't use VelocityDbList for collections and indexing.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   If an update of a persisted entity is attempted outside a session,
    ///   provides an meaningful error message.
    /// </exception>
    protected new void UpdateNonIndexField() {
      try {
        base.UpdateNonIndexField();
      } catch (NullReferenceException ex) {
        throw new InvalidOperationException(
          $"{EntityType.Name} '{SimpleKey}' cannot be updated outside a session " +
          "because it already exists.", ex);
      }
    }

    internal static void ValidateUrlFormat(string url, string propertyName) {
      try {
        var dummy = new Uri(url, UriKind.Absolute);
      } catch (UriFormatException) {
        throw new PropertyConstraintException(
          $"Invalid {propertyName} format: '{url}'.", propertyName);
      }
    }
  }
}