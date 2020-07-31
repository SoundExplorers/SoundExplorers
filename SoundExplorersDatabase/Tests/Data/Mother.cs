using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : RelativeBase {
    private string _name;

    public Mother() : base(typeof(Mother)) {
      Daughters = new SortedChildList<string, Daughter>(this);
      Sons = new SortedChildList<string, Son>(this);
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
    
    public SortedChildList<string, Son> Sons { get; }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return new[] {
        new ChildrenType(typeof(Daughter), Daughters),
        new ChildrenType(typeof(Son), Sons)
      };
    }

    protected override IEnumerable<Type> GetParentTypes() {
      return null;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      throw new NotSupportedException();
    }

    public static Mother Read(string name, SessionBase session) {
      return session.AllObjects<Mother>().First(mother => mother.Name == name);
    }
  }
}