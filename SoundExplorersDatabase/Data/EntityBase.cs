using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public abstract class EntityBase : ReferenceTracked, IEntity {
    private IDictionary<Type, IDictionary> _childrenOfType;
    private IDictionary<Type, IRelationInfo> _childrenRelations;
    private EntityBase _identifyingParent;
    private IDictionary<Type, IRelationInfo> _parentRelations;
    private IDictionary<Type, EntityBase> _parents;
    private QueryHelper _queryHelper;
    private Schema _schema;
    [CanBeNull] private string _simpleKey;

    protected EntityBase([NotNull] Type entityType,
      [NotNull] string simpleKeyName, [CanBeNull] Type identifyingParentType) {
      EntityType = entityType ??
                   throw new ArgumentNullException(
                     nameof(entityType));
      SimpleKeyName = simpleKeyName ??
                      throw new ArgumentNullException(nameof(simpleKeyName));
      IdentifyingParentType = identifyingParentType;
      Key = new Key(this);
    }

    [NotNull]
    private IDictionary<Type, IDictionary> ChildrenOfType {
      get {
        InitialiseIfNull(_childrenOfType);
        return _childrenOfType;
      }
      set {
        UpdateNonIndexField();
        _childrenOfType = value;
      }
    }

    [NotNull]
    private IDictionary<Type, IRelationInfo> ChildrenRelations {
      get {
        InitialiseIfNull(_childrenRelations);
        return _childrenRelations;
      }
      set {
        UpdateNonIndexField();
        _childrenRelations = value;
      }
    }

    /// <summary>
    ///   The Type as which the entity will be persisted on the database.
    ///   Entities of any subtypes will be members of the same
    ///   child collections, if any.
    /// </summary>
    [NotNull]
    internal Type EntityType { get; }

    [CanBeNull] private Type IdentifyingParentType { get; }
    private bool IsAddingToOrRemovingFromIdentifyingParent { get; set; }
    private bool IsTopLevel => Parents.Count == 0;

    [NotNull]
    private IDictionary<Type, IRelationInfo> ParentRelations {
      get {
        InitialiseIfNull(_parentRelations);
        return _parentRelations;
      }
      set {
        UpdateNonIndexField();
        _parentRelations = value;
      }
    }

    [NotNull]
    private IDictionary<Type, EntityBase> Parents {
      get {
        InitialiseIfNull(_parents);
        return _parents;
      }
      set {
        UpdateNonIndexField();
        _parents = value;
      }
    }

    [NotNull]
    internal QueryHelper QueryHelper {
      get => _queryHelper ?? (_queryHelper = QueryHelper.Instance);
      set => _queryHelper = value;
    }

    [NotNull]
    protected Schema Schema {
      get => _schema ?? (_schema = Schema.Instance);
      set => _schema = value;
    }

    [NotNull] private string SimpleKeyName { get; }

    [CanBeNull]
    public EntityBase IdentifyingParent {
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
          throw new NoNullAllowedException(
            "A null reference has been specified as the " +
            $"{IdentifyingParentType.Name} for {EntityType.Name} '{Key}'.");
        }
        if (value.EntityType != IdentifyingParentType) {
          throw new ConstraintException(
            $"A {value.EntityType.Name} has been specified as the " +
            $"IdentifyingParent for {EntityType.Name} '{Key}'. " +
            $"A {IdentifyingParentType.Name} is expected'.");
        }
        var newKey = new Key(SimpleKey, value);
        value.CheckForDuplicateChild(EntityType, newKey);
        if (_identifyingParent != null &&
            // Should always be true
            _identifyingParent.ChildrenOfType[EntityType].Contains(Key)) {
          _identifyingParent.ChildrenOfType[EntityType].Remove(Key);
          _identifyingParent.References.Remove(
            _identifyingParent.References.First(r => r.To.Equals(this)));
        }
        value.ChildrenOfType[EntityType].Add(newKey, this);
        value.References.AddFast(new Reference(this, "_children"));
        Parents[IdentifyingParentType] = value;
        _identifyingParent = value;
      }
    }

    [NotNull] public Key Key { get; }

    [CanBeNull]
    public string SimpleKey {
      get => _simpleKey;
      protected set =>
        _simpleKey = value ?? throw new NoNullAllowedException(
          $"A null reference has been specified as the {SimpleKeyName} " +
          $"for {EntityType.Name} '{Key}'. " +
          $"Null {SimpleKeyName}s are not supported.");
    }

    internal void AddChild([NotNull] EntityBase child) {
      CheckCanAddChild(child);
      ChildrenOfType[child.EntityType].Add(CreateChildKey(child), child);
      References.AddFast(new Reference(child, "_children"));
      UpdateChild(child, this);
    }

    protected void ChangeNonIdentifyingParent(
      [NotNull] Type parentEntityType,
      [CanBeNull] EntityBase newParent) {
      Parents[parentEntityType]?.RemoveChild(this, newParent != null);
      newParent?.AddChild(this);
      Parents[parentEntityType] = newParent;
    }

    private void CheckCanAddChild([NotNull] EntityBase child) {
      if (child == null) {
        throw new NoNullAllowedException(
          "A null reference has been specified. " +
          $"So addition to {EntityType.Name} '{Key}' is not supported.");
      }
      if (child.Parents[EntityType] != null) {
        throw new ConstraintException(
          $"{child.EntityType.Name} '{child.Key}' " +
          $"cannot be added to {EntityType.Name} '{Key}', " +
          $"because it already belongs to {EntityType.Name} " +
          $"'{child.Parents[EntityType].Key}'.");
      }
      CheckForDuplicateChild(child.EntityType, CreateChildKey(child));
    }

    protected virtual void CheckCanPersist([NotNull] SessionBase session) {
      if (SimpleKey == null) {
        throw new NoNullAllowedException(
          $"A {SimpleKeyName} has not yet been specified. " +
          $"So the {EntityType.Name} cannot be persisted.");
      }
      foreach (var parentKeyValuePair in Parents) {
        var parentType = parentKeyValuePair.Key;
        var parent = parentKeyValuePair.Value;
        if (parent == null && ParentRelations[parentType].IsMandatory) {
          throw new ConstraintException(
            $"{EntityType.Name} '{Key}' " +
            $"cannot be persisted because its {parentType.Name} "
            + "has not been specified.");
        }
      }
      if (IsTopLevel && IsDuplicateSimpleKey(session)) {
        throw new DuplicateKeyException(
          this,
          $"{EntityType.Name} '{Key}' " +
          $"cannot be persisted because another {EntityType.Name} "
          + "with the same key already persists.");
      }
    }

    private void CheckCanRemoveChild(
      [NotNull] EntityBase child, bool isReplacingOrUnpersisting) {
      if (child == null) {
        throw new NoNullAllowedException(
          "A null reference has been specified. " +
          $"So removal from {EntityType.Name} '{Key}' is not supported.");
      }
      if (!ChildrenOfType[child.EntityType].Contains(child.Key)) {
        throw new KeyNotFoundException(
          $"{child.EntityType.Name} '{child.Key}' " +
          $"cannot be removed from {EntityType.Name} '{Key}', " +
          $"because it does not belong to {EntityType.Name} " +
          $"'{Key}'.");
      }
      if (ChildrenRelations[child.EntityType].IsMandatory &&
          !isReplacingOrUnpersisting) {
        throw new ConstraintException(
          $"{child.EntityType.Name} '{child.Key}' " +
          $"cannot be removed from {EntityType.Name} '{Key}', " +
          "because membership is mandatory.");
      }
    }

    private void CheckForDuplicateChild([NotNull] Type childEntityType,
      [NotNull] Key keyToCheck) {
      if (ChildrenOfType[childEntityType]
        .Contains(keyToCheck)) {
        throw new DuplicateKeyException(
          keyToCheck,
          $"{childEntityType.Name} '{keyToCheck}' " +
          $"cannot be added to {EntityType.Name} '{Key}', " +
          $"because a {childEntityType.Name} with that Key " +
          $"already belongs to the {EntityType.Name}.");
      }
    }

    [NotNull]
    private Key CreateChildKey([NotNull] EntityBase child) {
      return new Key(child,
        child.IdentifyingParentType == EntityType ? this : null);
    }

    [NotNull]
    private IDictionary<Type, IRelationInfo> CreateChildrenRelations() {
      var values =
        from relation in Schema.Relations
        where relation.ParentType == EntityType
        select relation;
      return values.ToDictionary<RelationInfo, Type, IRelationInfo>(
        value => value.ChildType, value => value);
    }

    [NotNull]
    private IDictionary<Type, IRelationInfo> CreateParentRelations() {
      var values =
        from relation in Schema.Relations
        where relation.ChildType == EntityType
        select relation;
      return values.ToDictionary<RelationInfo, Type, IRelationInfo>(
        value => value.ParentType, value => value);
    }

    [NotNull]
    protected abstract IDictionary GetChildren([NotNull] Type childType);

    private void Initialise() {
      ParentRelations = CreateParentRelations();
      Parents = new Dictionary<Type, EntityBase>();
      foreach (var relationKvp in ParentRelations) {
        Parents.Add(relationKvp.Key, null);
      }
      ChildrenRelations = CreateChildrenRelations();
      ChildrenOfType = new Dictionary<Type, IDictionary>();
      foreach (var relationKvp in ChildrenRelations) {
        ChildrenOfType.Add(relationKvp.Key, GetChildren(relationKvp.Key));
      }
    }

    private void InitialiseIfNull([CanBeNull] object backingField) {
      if (backingField == null) {
        Initialise();
      }
    }

    private bool IsDuplicateSimpleKey([NotNull] SessionBase session) {
      var existing = QueryHelper.FindWithSameSimpleKey(this, session);
      return existing != null && !existing.Oid.Equals(Oid);
    }

    protected abstract void OnNonIdentifyingParentFieldToBeUpdated(
      [NotNull] Type parentEntityType, [CanBeNull] EntityBase newParent);

    public override ulong Persist(Placement place, SessionBase session,
      bool persistRefs = true,
      bool disableFlush = false,
      Queue<IOptimizedPersistable> toPersist = null) {
      CheckCanPersist(session);
      return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }

    internal void RemoveChild([NotNull] EntityBase child,
      bool isReplacingOrUnpersisting) {
      CheckCanRemoveChild(child, isReplacingOrUnpersisting);
      UpdateChild(child, null);
      ChildrenOfType[child.EntityType].Remove(child.Key);
      References.Remove(References.First(r => r.To.Equals(child)));
    }

    public override void Unpersist(SessionBase session) {
      var parents =
        Parents.Values.Where(parent => parent != null).ToList();
      for (int i = parents.Count - 1; i >= 0; i--) {
        parents[i].ChildrenOfType[EntityType].Remove(this);
        parents[i].RemoveChild(this, true);
      }
      base.Unpersist(session);
    }

    private void UpdateChild([NotNull] EntityBase child,
      [CanBeNull] EntityBase newParent) {
      child.UpdateNonIndexField();
      child.Parents[EntityType] = newParent;
      if (EntityType == child.IdentifyingParentType) {
        child.IsAddingToOrRemovingFromIdentifyingParent = true;
        child.IdentifyingParent = newParent;
        child.IsAddingToOrRemovingFromIdentifyingParent = false;
      } else {
        child.OnNonIdentifyingParentFieldToBeUpdated(EntityType,
          newParent);
      }
    }
  }
}