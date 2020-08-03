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
  public abstract class RelativeBase : ReferenceTracked {
    private IDictionary<Type, IDictionary> _childrenOfType;
    private IDictionary<Type, ChildrenRelation> _childrenRelations;
    private IDictionary<Type, ParentRelation> _parentRelations;
    private IDictionary<Type, RelativeBase> _parents;

    protected RelativeBase(Type persistableType) {
      PersistableType = persistableType;
    }

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

    private IDictionary<Type, ChildrenRelation> ChildrenRelations {
      get {
        InitialiseIfNull(_childrenRelations);
        return _childrenRelations;
      }
      set {
        UpdateNonIndexField();
        _childrenRelations = value;
      }
    }

    private IDictionary<Type, ParentRelation> ParentRelations {
      get {
        InitialiseIfNull(_parentRelations);
        return _parentRelations;
      }
      set {
        UpdateNonIndexField();
        _parentRelations = value;
      }
    }

    private bool IsTopLevel => Parents.Count == 0;

    public object Key { get; private set; }

    private IDictionary<Type, RelativeBase> Parents {
      get {
        InitialiseIfNull(_parents);
        return _parents;
      }
      set {
        UpdateNonIndexField();
        _parents = value;
      }
    }

    private Type PersistableType { get; }

    internal void AddChild([NotNull] RelativeBase child) {
      CheckCanAddChild(child);
      ChildrenOfType[child.PersistableType].Add(child.Key, child);
      References.AddFast(new Reference(child, "_children"));
      UpdateChild(child, this);
    }

    [CanBeNull]
    protected abstract IEnumerable<ChildrenType> GetChildrenTypes();

    [CanBeNull]
    protected abstract IEnumerable<ParentRelation> GetParentRelations();

    protected void ChangeParent(
      [NotNull] Type parentPersistableType,
      [CanBeNull] RelativeBase newParent) {
      Parents[parentPersistableType]?.RemoveChild(this, newParent != null);
      newParent?.AddChild(this);
      Parents[parentPersistableType] = newParent;
    }

    private void CheckCanAddChild([NotNull] RelativeBase child) {
      if (child == null) {
        throw new NoNullAllowedException(
          "A null reference has been specified. " +
          $"So addition to {PersistableType.Name} '{Key}' is not supported.");
      }
      if (child.Parents[PersistableType] != null) {
        throw new ConstraintException(
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be added to {PersistableType.Name} '{Key}', " +
          $"because it already belongs to {PersistableType.Name} " +
          $"'{child.Parents[PersistableType].Key}'.");
      }
      if (ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        throw new DuplicateKeyException(
          child,
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be added to {PersistableType.Name} '{Key}', " +
          "because it already belongs to it.");
      }
    }

    private void CheckCanPersist(SessionBase session) {
      if (Key == null) {
        throw new NoNullAllowedException(
          "A Key has not yet been specified. " +
          $"So the {PersistableType.Name} cannot be persisted.");
      }
      foreach (var parentKeyValuePair in Parents) {
        var parentType = parentKeyValuePair.Key;
        var parent = parentKeyValuePair.Value;
        if (parent == null && ParentRelations[parentType].IsMandatory) {
          throw new ConstraintException(
            $"{PersistableType.Name} '{Key}' " +
            $"cannot be persisted because its {parentType.Name} "
            + "has not been specified.");
        }
      }
      if (IsTopLevel) {
        //FindWithSameKey(session);
      }
    }

    private void CheckCanRemoveChild([NotNull] RelativeBase child,
      bool isReplacingOrUnpersisting) {
      if (child == null) {
        throw new NoNullAllowedException(
          "A null reference has been specified. " +
          $"So removal from {PersistableType.Name} '{Key}' is not supported.");
      }
      if (!ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        throw new KeyNotFoundException(
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be removed from {PersistableType.Name} '{Key}', " +
          $"because it does not belong to {PersistableType.Name} " +
          $"'{Key}'.");
      }
      if (ChildrenRelations[child.PersistableType].IsMembershipMandatory &&
          !isReplacingOrUnpersisting) {
        throw new ConstraintException(
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be removed from {PersistableType.Name} '{Key}', " +
          "because membership is mandatory.");
      }
    }

    [CanBeNull]
    protected abstract RelativeBase FindWithSameKey(
      [NotNull] SessionBase session);

    private void Initialise() {
      var parentRelations = GetParentRelations();
      var childrenTypes = GetChildrenTypes();
      Parents = new Dictionary<Type, RelativeBase>();
      ParentRelations = new Dictionary<Type, ParentRelation>();
      if (parentRelations != null) {
        foreach (var parentRelation in parentRelations) {
          Parents.Add(parentRelation.ParentType, null);
          ParentRelations.Add(parentRelation.ParentType, parentRelation);
        }
      }
      ChildrenOfType = new Dictionary<Type, IDictionary>();
      ChildrenRelations = new Dictionary<Type, ChildrenRelation>();
      if (childrenTypes != null) {
        foreach (var childrenType in childrenTypes) {
          ChildrenOfType.Add(childrenType.ChildType, childrenType.Children);
          ChildrenRelations.Add(childrenType.ChildType, childrenType);
        }
      }
    }

    private void InitialiseIfNull([CanBeNull] object field) {
      if (field == null) {
        Initialise();
      }
    }

    protected abstract void OnParentFieldToBeUpdated(
      [NotNull] Type parentPersistableType, [CanBeNull] RelativeBase newParent);

    public override ulong Persist(Placement place, SessionBase session,
      bool persistRefs = true,
      bool disableFlush = false,
      Queue<IOptimizedPersistable> toPersist = null) {
      CheckCanPersist(session);
      return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }

    internal void RemoveChild([NotNull] RelativeBase child,
      bool isReplacingOrUnpersisting) {
      CheckCanRemoveChild(child, isReplacingOrUnpersisting);
      UpdateChild(child, null);
      ChildrenOfType[child.PersistableType].Remove(child.Key);
      References.Remove(References.First(r => r.To.Equals(child)));
    }

    protected virtual void SetKey([NotNull] object value) {
      Key = value ?? throw new NoNullAllowedException(
        "A null reference has been specified as the Key " +
        $"for {PersistableType.Name} '{Key}'. " +
        "Null Keys are not supported.");
    }

    public override void Unpersist(SessionBase session) {
      var parents =
        Parents.Values.Where(parent => parent != null).ToList();
      for (int i = parents.Count - 1; i >= 0; i--) {
        parents[i].ChildrenOfType[PersistableType].Remove(this);
        parents[i].RemoveChild(this, true);
      }
      base.Unpersist(session);
    }

    private void UpdateChild([NotNull] RelativeBase child,
      [CanBeNull] RelativeBase newParent) {
      child.UpdateNonIndexField();
      child.Parents[PersistableType] = newParent;
      child.OnParentFieldToBeUpdated(PersistableType, newParent);
    }
  }
}