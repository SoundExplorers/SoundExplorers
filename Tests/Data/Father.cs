using System;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Father : EntityBase {
    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Father(SortedEntityCollection<Father> root, QueryHelper queryHelper) : base(
      root, typeof(Father),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedEntityCollection<Daughter>();
      Sons = new SortedEntityCollection<Son>();
    }

    public SortedEntityCollection<Daughter> Daughters { get; }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    public SortedEntityCollection<Son> Sons { get; }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }
  }
}