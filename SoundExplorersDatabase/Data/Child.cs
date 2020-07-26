using System.Linq;
using VelocityDb;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Child : OptimizedPersistable {
    private string _name;
    private Parent _parent;

    internal bool IsChangingParent { get; private set; }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
      }
    }

    public Parent Parent {
      get => _parent;
      set {
        if (_parent != null && !_parent.IsChangingChildren
            || value != null && !value.IsChangingChildren) {
          IsChangingParent = true;
        }
        if (_parent != null && !_parent.IsChangingChildren) {
          _parent.Children.Remove(this);
        }
        if (value != null && !value.IsChangingChildren) {
          value.Children.Add(this);
        }
        UpdateNonIndexField();
        _parent = value;
        IsChangingParent = false;
      }
    }

    public static Child Read(string name, SessionBase session) {
      return session.AllObjects<Child>().First(child => child.Name == name);
    }

    public override void Unpersist(SessionBase session) {
      Parent?.Children.Remove(this);
      base.Unpersist(session);
    }
  }
}