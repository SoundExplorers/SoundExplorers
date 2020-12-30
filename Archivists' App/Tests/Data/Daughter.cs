using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Daughter : EntityBase {
    private Father _father = null!;
    private Mother _mother = null!;

    public Daughter([JetBrains.Annotations.NotNull] QueryHelper queryHelper,
      Type? identifyingParentType = null) : base(typeof(Daughter),
      nameof(Name), identifyingParentType) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
    }

    [CanBeNull]
    public Father Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Father), value);
        _father = value;
      }
    }

    [CanBeNull]
    public virtual Mother Mother {
      get => _mother;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Mother), value);
        _mother = value;
      }
    }

    public string? Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType,
      EntityBase newParent) {
      if (parentEntityType == typeof(Father)) {
        _father = (Father)newParent;
      } else {
        _mother = (Mother)newParent;
      }
    }
  }
}