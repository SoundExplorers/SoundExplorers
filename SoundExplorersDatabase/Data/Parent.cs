using System.Linq;
using System.Reflection;
using VelocityDb;
using VelocityDb.Collection.BTree;
using VelocityDb.Collection.BTree.Extensions;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Parent : ReferenceTracked {
    private ParentChildren _children;
    [Index] [UniqueConstraint] private string _name;

    internal bool IsChangingChildren { get; private set; }

    public ParentChildren Children {
      get {
        if (_children == null) {
          Update();
          _children = new ParentChildren(this, GetAddChildMethod(),
            GetRemoveChildMethod());
        }
        return _children;
      }
    }

    [FieldAccessor("_name")]
    public string Name {
      get => _name;
      set {
        Update();
        _name = value;
      }
    }

    public static Parent Read(string name, SessionBase session) {
      // ReSharper disable once ReplaceWithSingleCallToFirst
      return session.Index<Parent>("_name")
        .Where(parent => parent.Name == name).First();
    }

    public override void Unpersist(SessionBase session) {
      Children.Unpersist(session);
      base.Unpersist(session);
    }

    private bool AddChild(Child child) {
      IsChangingChildren = true;
      bool result = ((BTreeSet<Child>)Children).Add(child);
      if (result) {
        References.AddFast(new Reference(child, "_children"));
        if (!child.IsChangingParent) {
          child.Parent = this;
        }
      }
      IsChangingChildren = false;
      return result;
    }

    private MethodInfo GetAddChildMethod() {
      return GetType().GetMethod(nameof(AddChild),
        BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private MethodInfo GetRemoveChildMethod() {
      return GetType().GetMethod(nameof(RemoveChild),
        BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private bool RemoveChild(Child child) {
      IsChangingChildren = true;
      if (!child.IsChangingParent) {
        child.Parent = null;
      }
      bool result = ((BTreeSet<Child>)Children).Remove(child);
      if (result) {
        References.Remove(References.First(r => r.To.Equals(child)));
      }
      IsChangingChildren = false;
      return result;
    }
  }
}