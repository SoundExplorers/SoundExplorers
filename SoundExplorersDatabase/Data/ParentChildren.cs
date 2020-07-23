using System.Reflection;
using VelocityDb.Collection.BTree;

namespace SoundExplorersDatabase.Data {
  public class ParentChildren : BTreeSet<Child> {
    public ParentChildren(Parent parent, MethodInfo addChild, MethodInfo removeChild) {
      Parent = parent;
      AddChild = addChild;
      RemoveChild = removeChild;
    }

    private MethodInfo AddChild { get; }
    private Parent Parent { get; }
    private MethodInfo RemoveChild { get; }

    public new bool Add(Child child) {
      return (bool)AddChild.Invoke(Parent, new object[] {child});
    }

    public new bool Remove(Child child) {
      return (bool)RemoveChild.Invoke(Parent, new object[] {child});
    }
  }
}