using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Son : RelativeBase {
    private Father _father;
    private Mother _mother;
    private string _name;

    static Son() {
      ParentRelations = new Dictionary<Type, ParentRelation> {
        {typeof(Father), new ParentRelation(typeof(Father), false)},
        {typeof(Mother), new ParentRelation(typeof(Mother), false)}
      };
    }

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

    [NotNull]
    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        SetKey(value);
      }
    }

    [NotNull]
    public static IDictionary<Type, ParentRelation> ParentRelations { get; }

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey(SessionBase session) {
      throw new NotSupportedException();
    }

    protected override IEnumerable<ChildrenType> GetChildrenTypes() {
      return null;
    }

    protected override IEnumerable<ParentRelation> GetParentRelations() {
      return new[] {
        ParentRelations[typeof(Father)],
        ParentRelations[typeof(Mother)]
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

    public static Son Read([NotNull] string name,
      [NotNull] SessionBase session) {
      return session.AllObjects<Son>().First(son => son.Name == name);
    }
  }
}