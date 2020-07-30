using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Child : RelativeBase<Child> {
    private string _name;
    private Parent _parent;

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        Key = value;
      }
    }

    public Parent Parent {
      get => _parent;
      set {
        UpdateNonIndexField();
        SetParent<Parent>(value);
        _parent = value;
      }
    }

    public static Child Read(string name, SessionBase session) {
      return session.AllObjects<Child>().First(child => child.Name == name);
    }

    public override void OnParentToBeUpdated(Type parentType, IRelativeBase newParent) {
      if (parentType != typeof(Parent)) {
        throw new ArgumentException($"Parent type {parentType} is invalid.", nameof(parentType));
      }
      Parent = (Parent)newParent;
    }

    protected override IEnumerable<IChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return new[] {typeof(Parent)};
    }
  }
}