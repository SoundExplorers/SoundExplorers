using System;
using System.Collections.Generic;
using System.Linq;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Son : RelativeBase {
    private Father _father;
    private Mother _mother;
    private string _name;

    public Son() : base(typeof(Son)) { }

    public Father Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Father), value);
        _father = value;
      }
    }

    public Mother Mother {
      get => _mother;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Mother), value);
        _mother = value;
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

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return new[] {
        typeof(Father),
        typeof(Mother)
      };
    }

    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Father)) {
        _father = (Father)newParent;
      } else {
        _mother = (Mother)newParent;
      }
    }

    public static Son Read(string name, SessionBase session) {
      return session.AllObjects<Son>().First(son => son.Name == name);
    }
  }
}