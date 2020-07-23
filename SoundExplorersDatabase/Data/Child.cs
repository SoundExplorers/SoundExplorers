using System.Collections.Generic;
using System.Linq;
using VelocityDb;
using VelocityDb.Collection.BTree.Extensions;
using VelocityDb.Indexing;
using VelocityDb.Session;
using VelocityDb.TypeInfo;

namespace SoundExplorersDatabase.Data {
  public class Child : OptimizedPersistable {
    [Index] [UniqueConstraint] private string _name;
    private Parent _parent;

    internal bool IsChangingParent { get; private set; }
    private Parent ParentToMoveFrom { get; set; }

    [FieldAccessor("_name")]
    public string Name {
      get => _name;
      set {
        Update();
        _name = value;
      }
    }

    public Parent Parent {
      get => _parent;
      set {
        if (value != null && value.Equals(_parent)) {
          return;
        }
        _parent = value;
        if (Parent != null && !Parent.IsChangingChildren) {
          ParentToMoveFrom = _parent;
          IsChangingParent = true;
          if (IsPersistent) {
            UpdateOldAndNewParents();
          }
        }
        Update();
        _parent = value;
      }
    }

    public override ulong Persist(Placement place, SessionBase session,
      bool persistRefs = true,
      bool disableFlush = false, Queue<IOptimizedPersistable> toPersist = null) {
      ulong result = base.Persist(place, session, persistRefs, disableFlush, toPersist);
      if (IsChangingParent) {
        UpdateOldAndNewParents();
      }
      return result;
    }

    public static Child Read(string name, SessionBase session) {
      // ReSharper disable once ReplaceWithSingleCallToFirst
      return session.Index<Child>("_name")
        .Where(child => child.Name == name).First();
    }

    private void UpdateOldAndNewParents() {
      ParentToMoveFrom?.Children.Remove(this);
      Parent?.Children.Add(this);
      ParentToMoveFrom = null;
      IsChangingParent = false;
    }
  }
}