using System;
using System.Collections;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Mother : EntityBase {
    public Mother([NotNull] QueryHelper queryHelper) : base(
      typeof(Mother),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>();
      Sons = new SortedChildList<Son>();
    }

    [NotNull] public SortedChildList<Daughter> Daughters { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    [NotNull] public SortedChildList<Son> Sons { get; }

    protected override IDictionary GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }
  }
}