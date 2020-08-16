using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Father : RelativeBase {
    public Father([NotNull] QueryHelper queryHelper) : base(typeof(Father),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>(this);
      Sons = new SortedChildList<Son>(this);
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

    protected override RelativeBase FindWithSameKey(SessionBase session) {
      return QueryHelper.Find<Father>(SimpleKey, session);
    }

    protected override IDictionary GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }

      return Sons;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}