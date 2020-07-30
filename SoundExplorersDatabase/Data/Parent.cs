using System;
using System.Collections.Generic;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Parent : RelativeBase {
    private string _name;

    public Parent() : base(typeof(Parent)) {
      Children = new SortedChildList<string, Child>(this);
    }

    public SortedChildList<string, Child> Children { get; }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        Key = value;
      }
    }

    protected override IEnumerable<IChildrenType> GetChildrenTypes() {
      return new[] {new ChildrenType<Child>(Children)};
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return null;
    }

    protected override void OnParentFieldToBeUpdated(RelativeBase newParent) {
      throw new NotSupportedException();
    }

    public static Parent Read(string name, SessionBase session) {
      return session.AllObjects<Parent>().First(parent => parent.Name == name);
    }
  }
}