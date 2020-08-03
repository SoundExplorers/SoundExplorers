using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : RelativeBase {
    private string _name;

    public Mother() : base(typeof(Mother)) {
      Daughters = new SortedChildList<string, Daughter>(this);
      Sons = new SortedChildList<string, Son>(this);
    }

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

    protected override IDictionary GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
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