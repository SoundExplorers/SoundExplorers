using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : EntityBase {
    public Mother([NotNull] QueryHelper queryHelper) : base(typeof(Mother),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>(this);
      Sons = new SortedChildList<Son>(this);
    }

    [NotNull] public SortedChildList<Daughter> Daughters { get; }

    [CanBeNull]
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

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType,
      EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}