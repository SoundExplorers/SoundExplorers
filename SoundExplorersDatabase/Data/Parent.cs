using System.Collections.Generic;
using System.Linq;
using VelocityDb;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Parent : ReferenceTracked {
    private readonly ParentChildren _children;
    private string _name;

    public Parent() {
      _children = new ParentChildren(this);
    }

    // ReSharper disable once ConvertToAutoProperty
    public ParentChildren Children => _children;
    internal bool IsChangingChildren { get; private set; }

    public string Name {
      get => _name;
      set {
        Update();
        _name = value;
      }
    }

    public static Parent Read(string name, SessionBase session) {
      return session.AllObjects<Parent>().First(parent => parent.Name == name);
    }

    internal bool AddChild(Child child) {
      IsChangingChildren = true;
      var result = false;
      if (!Children.ContainsKey(child.Name)) {
        ((SortedList<string, Child>)Children).Add(child.Name, child);
        result = true;
      }
      if (result) {
        References.AddFast(new Reference(child, "_children"));
        if (!child.IsChangingParent) {
          child.Parent = this;
        }
      }
      IsChangingChildren = false;
      return result;
    }

    internal bool RemoveChild(Child child) {
      IsChangingChildren = true;
      if (!child.IsChangingParent) {
        child.Parent = null;
      }
      var result = false;
      if (Children.ContainsKey(child.Name)) {
        ((SortedList<string, Child>)Children).Remove(child.Name);
        result = true;
      }
      if (result) {
        References.Remove(References.First(r => r.To.Equals(child)));
      }
      IsChangingChildren = false;
      return result;
    }
  }
}