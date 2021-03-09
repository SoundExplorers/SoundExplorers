using System;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Mother : EntityBase {
    public Mother(QueryHelper queryHelper) : base(
      typeof(Mother),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>();
      Sons = new SortedChildList<Son>();
    }

    public SortedChildList<Daughter> Daughters { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    public SortedChildList<Son> Sons { get; }

    protected override ISortedChildList GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }
  }
}