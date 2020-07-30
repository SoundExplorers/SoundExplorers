using System;
using System.Collections.Generic;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Parent : RelativeBase<Parent> {
    public Parent() {
      _children = new SortedChildList<string, Child>(this);
    }
    private readonly SortedChildList<string, Child> _children;
    private string _name;
    
    // ReSharper disable once ConvertToAutoProperty
    public SortedChildList<string, Child> Children => _children;

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

    public override void OnParentToBeUpdated(Type parentType, IRelativeBase newParent) {
      throw new NotSupportedException();
    }

    protected override IEnumerable<IChildrenType> GetChildrenTypes() {
      return new[] {new ChildrenType<Child>(Children)};
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return null;
    }
  }
}