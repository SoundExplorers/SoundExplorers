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
    private IDictionary<Type, RelativeBase> _parentOfType;

    protected RelativeBase(Type persistableType) {
      PersistableType = persistableType;
    }

    private IDictionary<Type, IDictionary> ChildrenOfType {
      get {
        if (_childrenOfType == null) {
          Initialise();
        }
        return _childrenOfType;
      }
      set {
        UpdateNonIndexField();
        _childrenOfType = value;
      }
    }

    private bool IsTopLevel => ParentOfType.Count == 0;
    
    [NotNull]
    public object Key { get; private set; }

    private IDictionary<Type, RelativeBase> ParentOfType {
      get {
        if (_parentOfType == null) {
          Initialise();
        }
        return _parentOfType;
      }
      set {
        UpdateNonIndexField();
        _parentOfType = value;
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
    protected abstract IEnumerable<Type> GetParentTypes();

    protected void ChangeParent(
      [NotNull] Type parentPersistableType,
      [CanBeNull] RelativeBase newParent) {
      ParentOfType[parentPersistableType]?.RemoveChild(this);
      newParent?.AddChild(this);
      ParentOfType[parentPersistableType] = newParent;
    }

    private void CheckCanAddChild([NotNull] RelativeBase child) {
      if (child == null) {
        throw new NoNullAllowedException(
          $"A null reference has been specified. " +
          $"So addition to {PersistableType.Name} '{Key}' is not supported.");
      }
      if (child.ParentOfType[PersistableType] != null) {
        throw new ConstraintException(
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be added to {PersistableType.Name} '{Key}', " +
          $"because it already belongs to {PersistableType.Name} " +
          $"'{child.ParentOfType[PersistableType].Key}'.");
      }
      if (ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        throw new DuplicateKeyException(
          child,
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be added to {PersistableType.Name} '{Key}', " +
          $"because it already belongs to it.");
      }
    }

    private void CheckCanPersist(SessionBase session) {
      if (Key == null) {
        throw new NoNullAllowedException(
          "A Key has not yet been specified. " + 
          $"So the {PersistableType.Name} cannot be persisted.");
      }
      if (IsTopLevel) {
        //FindWithSameKey(session);
      }
    }

    private void CheckCanRemoveChild([NotNull] RelativeBase child) {
      if (child == null) {
        throw new NoNullAllowedException(
          $"A null reference has been specified. " +
          $"So removal from {PersistableType.Name} '{Key}' is not supported.");
      }
      if (!ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        throw new KeyNotFoundException(
          $"{child.PersistableType.Name} '{child.Key}' " +
          $"cannot be removed from {PersistableType.Name} '{Key}', " +
          $"because it does not belong to {PersistableType.Name} " +
          $"'{Key}'.");
      }
    }

    [CanBeNull]
    protected abstract RelativeBase FindWithSameKey([NotNull] SessionBase session);

    private void Initialise() {
      var parentTypes = GetParentTypes();
      var childrenTypes = GetChildrenTypes();
      ParentOfType = new Dictionary<Type, RelativeBase>();
      if (parentTypes != null) {
        foreach (var parentType in parentTypes) {
          ParentOfType.Add(parentType, null);
        }
      }
      ChildrenOfType = new Dictionary<Type, IDictionary>();
      if (childrenTypes != null) {
        foreach (var childrenType in childrenTypes) {
          ChildrenOfType.Add(childrenType.ChildType, childrenType.Children);
        }
      }
    }

    protected abstract void OnParentFieldToBeUpdated(
      [NotNull] Type parentPersistableType, [CanBeNull] RelativeBase newParent);

    public override ulong Persist(Placement place, SessionBase session, bool persistRefs = true,
      bool disableFlush = false, Queue<IOptimizedPersistable> toPersist = null) {
      CheckCanPersist(session);
      return base.Persist(place, session, persistRefs, disableFlush, toPersist);
    }

    internal void RemoveChild([NotNull] RelativeBase child) {
      CheckCanRemoveChild(child);
      UpdateChild(child, null);
      ChildrenOfType[child.PersistableType].Remove(child.Key);
      References.Remove(References.First(r => r.To.Equals(child)));
    }

    protected virtual void SetKey([NotNull] object value) {
      Key = value ?? throw new NoNullAllowedException(
              $"A null reference has been specified as the Key " + 
              $"for {PersistableType.Name} '{Key}'. " +
              "Null Keys are not supported.");
    }

    public override void Unpersist(SessionBase session) {
      var parents =
        ParentOfType.Values.Where(parent => parent != null).ToList();
      for (int i = parents.Count - 1; i >= 0; i--) {
        parents[i].ChildrenOfType[PersistableType].Remove(this);
        parents[i].RemoveChild(this);
      }
      base.Unpersist(session);
    }

    private void UpdateChild([NotNull] RelativeBase child,
      [CanBeNull] RelativeBase newParent) {
      child.UpdateNonIndexField();
      child.ParentOfType[PersistableType] = newParent;
      child.OnParentFieldToBeUpdated(PersistableType, newParent);
    }
  }
}