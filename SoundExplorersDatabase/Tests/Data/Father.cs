using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Father : RelativeBase {
    private string _name;

    public Father() : base(typeof(Father)) {
      Daughters = new SortedChildList<string, Daughter>(this, false);
      Sons = new SortedChildList<string, Son>(this, false);
    }

    public SortedChildList<string, Daughter> Daughters { get; }

    [NotNull]
    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        SetKey(value);
      }
    }

    public SortedChildList<string, Son> Sons { get; }

    protected override RelativeBase FindWithSameKey(SessionBase session) {
      return session.AllObjects<Father>()
        .FirstOrDefault(father => father.Name == Name);
    }

    protected override IEnumerable<ChildrenRelation> GetChildrenRelations() {
      return new[] {
        new ChildrenRelation(typeof(Daughter), Daughters),
        new ChildrenRelation(typeof(Son), Sons)
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

    public static Father Read([NotNull] string name,
      [NotNull] SessionBase session) {
      return session.AllObjects<Father>().First(father => father.Name == name);
    }
  }
}