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
    private IDictionary<Type, bool> _isChangingChildrenOfType;
    private IDictionary<Type, bool> _isChangingParentOfType;
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

    private IDictionary<Type, bool> IsChangingChildrenOfType {
      get {
        if (_isChangingChildrenOfType == null) {
          Initialise();
        }
        return _isChangingChildrenOfType;
      }
      set {
        UpdateNonIndexField();
        _isChangingChildrenOfType = value;
      }
    }

    private IDictionary<Type, bool> IsChangingParentOfType {
      get {
        if (_isChangingParentOfType == null) {
          Initialise();
        }
        return _isChangingParentOfType;
      }
      set {
        UpdateNonIndexField();
        _isChangingParentOfType = value;
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
      IsChangingChildrenOfType[child.PersistableType] = true;
      var result = false;
      if (!ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        ChildrenOfType[child.PersistableType].Add(child.Key, child);
        result = true;
      }
      if (result) {
        References.AddFast(new Reference(child, "_children"));
        if (!child.IsChangingParentOfType[PersistableType]) {
          child.ParentOfType[PersistableType] = this;
          child.OnParentToBeUpdated(PersistableType, this);
        }
      }
      IsChangingChildrenOfType[child.PersistableType] = false;
      return result;
    }

    [CanBeNull]
    protected abstract IEnumerable<IChildrenType> GetChildrenTypes();

    [CanBeNull]
    protected abstract IEnumerable<Type> GetParentTypes();

    private void Initialise() {
      var parentTypes = GetParentTypes();
      var childrenTypes = GetChildrenTypes();
      ParentOfType = new Dictionary<Type, RelativeBase>();
      IsChangingParentOfType = new Dictionary<Type, bool>();
      if (parentTypes != null) {
        foreach (var parentType in parentTypes) {
          ParentOfType.Add(parentType, null);
          IsChangingParentOfType.Add(parentType, false);
        }
      }
      ChildrenOfType = new Dictionary<Type, IDictionary>();
      IsChangingChildrenOfType = new Dictionary<Type, bool>();
      if (childrenTypes != null) {
        foreach (var childrenType in childrenTypes) {
          ChildrenOfType.Add(childrenType.ChildType, childrenType.Children);
          IsChangingChildrenOfType.Add(childrenType.ChildType, false);
        }
      }
    }

    protected abstract void OnParentToBeUpdated(Type parentType,
      RelativeBase newParent);

    internal bool RemoveChild(RelativeBase child) {
      ValidateChild(child);
      IsChangingChildrenOfType[child.PersistableType] = true;
      if (!child.IsChangingParentOfType[PersistableType]) {
        child.ParentOfType[PersistableType] = null;
        child.OnParentToBeUpdated(PersistableType, null);
      }
      var result = false;
      if (ChildrenOfType[child.PersistableType].Contains(child.Key)) {
        ChildrenOfType[child.PersistableType].Remove(child.Key);
        result = true;
      }
      if (result) {
        References.Remove(References.First(r => r.To.Equals(child)));
      }
      IsChangingChildrenOfType[child.PersistableType] = false;
      return result;
    }

    protected void SetParent<TParent>([CanBeNull] TParent value)
      where TParent : RelativeBase {
      var parentType = typeof(TParent);
      IsChangingParentOfType[parentType] =
        ParentOfType[parentType] != null &&
        !ParentOfType[parentType].IsChangingChildrenOfType[PersistableType] ||
        value != null && !value.IsChangingChildrenOfType[PersistableType];
      if (ParentOfType[parentType] != null &&
          !ParentOfType[parentType].IsChangingChildrenOfType[PersistableType]) {
        ParentOfType[parentType].ChildrenOfType[PersistableType].Remove(Key);
        ParentOfType[parentType].RemoveChild(this);
      }
      if (value != null && !value.IsChangingChildrenOfType[PersistableType]) {
        value.ChildrenOfType[PersistableType].Add(Key, this);
      }
      UpdateNonIndexField();
      ParentOfType[parentType] = value;
      IsChangingParentOfType[parentType] = false;
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