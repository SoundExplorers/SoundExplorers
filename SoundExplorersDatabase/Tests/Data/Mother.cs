using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : RelativeBase {
    private string _name;

    static Mother() {
      ChildrenRelations = new Dictionary<Type, ChildrenRelation> {
        {typeof(Daughter), new ChildrenRelation(typeof(Daughter), true)},
        {typeof(Son), new ChildrenRelation(typeof(Son), false)}
      };
    }

    public Mother() : base(typeof(Mother)) {
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
      return session.AllObjects<Mother>()
        .FirstOrDefault(mother => mother.Name == Name);
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

    public static Mother Read([NotNull] string name,
      [NotNull] SessionBase session) {
      return session.AllObjects<Mother>().First(mother => mother.Name == name);
    }
  }
}