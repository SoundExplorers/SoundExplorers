using System;
using System.Collections.Generic;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Child : RelativeBase {
    private string _name;
    private Parent _parent;

    public Child() : base(typeof(Child)) { }

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
        SetParent(value);
        _parent = value;
      }
    }

    protected override IEnumerable<IChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return new[] {typeof(Parent)};
    }

    protected override void OnParentToBeUpdated(Type parentType,
      RelativeBase newParent) {
      if (parentType != typeof(Parent)) {
        throw new ArgumentException($"Parent type {parentType} is invalid.",
          nameof(parentType));
      }
      Parent = (Parent)newParent;
    }

    public static Child Read(string name, SessionBase session) {
      return session.AllObjects<Child>().First(child => child.Name == name);
    }
  }
}