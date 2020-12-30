using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Father : EntityBase {
    public Father([JetBrains.Annotations.NotNull] QueryHelper queryHelper) : base(
      typeof(Father),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>();
      Sons = new SortedChildList<Son>();
    }

    [JetBrains.Annotations.NotNull] public SortedChildList<Daughter> Daughters { get; }

    [CanBeNull]
    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    [JetBrains.Annotations.NotNull] public SortedChildList<Son> Sons { get; }

    protected override IDictionary GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType,
      EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}