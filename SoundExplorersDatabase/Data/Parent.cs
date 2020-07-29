using System;
using System.Collections.Generic;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Parent : RelativeBase<Parent> {
    private SortedChildList<string, Child> _children;
    private string _name;

    public SortedChildList<string, Child> Children {
      get {
        if (_children == null) {
          UpdateNonIndexField();
          _children = new SortedChildList<string, Child>(this);
        }
        return _children;
      }
    }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        Key = value;
      }
    }

    public static Parent Read(string name, SessionBase session) {
      return session.AllObjects<Parent>().First(parent => parent.Name == name);
    }

    protected override IEnumerable<IChildrenType> GetChildrenTypes() {
      return new[] {new ChildrenType<Child>(Children)};
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return null;
    }
  }
}