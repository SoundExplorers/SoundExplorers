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

    static Father() {
      ChildrenRelations = new Dictionary<Type, ChildrenRelation> {
        {typeof(Daughter), new ChildrenRelation(typeof(Daughter), false)},
        {typeof(Son), new ChildrenRelation(typeof(Son), false)}
      };
    }

    public Father() : base(typeof(Father)) {
      Daughters = new SortedChildList<string, Daughter>(this);
      Sons = new SortedChildList<string, Son>(this);
    }

    [NotNull]
    public static IDictionary<Type, ChildrenRelation> ChildrenRelations { get; }

    [NotNull] public SortedChildList<string, Daughter> Daughters { get; }

    [NotNull]
    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        SetKey(value);
      }
    }

    [NotNull] public SortedChildList<string, Son> Sons { get; }

    protected override RelativeBase FindWithSameKey(SessionBase session) {
      return session.AllObjects<Father>()
        .FirstOrDefault(father => father.Name == Name);
    }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return new[] {
        new ChildrenType(ChildrenRelations[typeof(Daughter)], Daughters),
        new ChildrenType(ChildrenRelations[typeof(Son)], Sons)
      };
    }

    protected override IEnumerable<ParentRelation> GetParentRelations() {
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