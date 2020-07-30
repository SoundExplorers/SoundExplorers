using System;
using System.Collections;
using System.Collections.Generic;
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

    protected object Key { get; set; }

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

    internal bool AddChild(RelativeBase child) {
      ValidateChild(child);
      var result = false;
      if (!ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        ChildrenOfType[child.PersistableType].Add(child.Key, child);
        result = true;
      }
      if (result) {
        References.AddFast(new Reference(child, "_children"));
        child.ParentOfType[PersistableType] = this;
        child.OnParentFieldToBeUpdated(this);
      }
      return result;
    }

    [CanBeNull]
    protected abstract IEnumerable<ChildrenType> GetChildrenTypes();

    [CanBeNull]
    protected abstract IEnumerable<Type> GetParentTypes();

    protected void ChangeParent<TParent>([CanBeNull] TParent value)
      where TParent : RelativeBase {
      var parentType = typeof(TParent);
      ParentOfType[parentType]?.RemoveChild(this);
      value?.AddChild(this);
      ParentOfType[parentType] = value;
    }

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

    protected abstract void OnParentFieldToBeUpdated(RelativeBase newParent);

    internal bool RemoveChild([NotNull] RelativeBase child) {
      ValidateChild(child);
      child.ParentOfType[PersistableType] = null;
      child.OnParentFieldToBeUpdated(null);
      var result = false;
      if (ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        ChildrenOfType[child.PersistableType].Remove(child.Key);
        result = true;
      }
      if (result) {
        References.Remove(References.First(r => r.To.Equals(child)));
      }
      return result;
    }

    private void ValidateChild(RelativeBase child) {
      if (!ChildrenOfType.ContainsKey(child.PersistableType)) {
        throw new ArgumentException(
          $"Children of type {child.PersistableType} are not supported.",
          nameof(child));
      }
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
  }
}