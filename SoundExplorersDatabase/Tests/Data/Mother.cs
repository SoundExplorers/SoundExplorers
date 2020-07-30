using System;
using System.Collections.Generic;
using System.Linq;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : RelativeBase {
    private string _name;

    public Mother() : base(typeof(Mother)) {
      Daughters = new SortedChildList<string, Daughter>(this);
    }

    public SortedChildList<string, Daughter> Daughters { get; }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        Key = value;
      }
    }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return new[] {new ChildrenType(typeof(Daughter), Daughters)};
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return null;
    }

    protected override void OnParentFieldToBeUpdated(RelativeBase newParent) {
      throw new NotSupportedException();
    }

    public static Mother Read(string name, SessionBase session) {
      return session.AllObjects<Mother>().First(parent => parent.Name == name);
    }
  }
}