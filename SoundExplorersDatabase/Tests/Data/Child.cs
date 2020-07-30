using System;
using System.Collections.Generic;
using System.Linq;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
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
        ChangeParent(value);
        _parent = value;
      }
    }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return new[] {typeof(Parent)};
    }

    protected override void OnParentFieldToBeUpdated(RelativeBase newParent) {
      _parent = (Parent)newParent;
    }

    public static Child Read(string name, SessionBase session) {
      return session.AllObjects<Child>().First(child => child.Name == name);
    }
  }
}