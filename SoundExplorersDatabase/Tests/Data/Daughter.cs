using System;
using System.Collections.Generic;
using System.Linq;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Daughter : RelativeBase {
    private string _name;
    private Mother _mother;

    public Daughter() : base(typeof(Daughter)) { }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        Key = value;
      }
    }

    public Mother Mother {
      get => _mother;
      set {
        UpdateNonIndexField();
        ChangeParent(value);
        _mother = value;
      }
    }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return new[] {typeof(Mother)};
    }

    protected override void OnParentFieldToBeUpdated(RelativeBase newParent) {
      _mother = (Mother)newParent;
    }

    public static Daughter Read(string name, SessionBase session) {
      return session.AllObjects<Daughter>().First(child => child.Name == name);
    }
  }
}