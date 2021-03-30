using System;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Son : EntityBase {
    private Father? _father;
    private Mother? _mother;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Son(SortedEntityCollection<Son> root, QueryHelper queryHelper) : base(
      root, typeof(Son),
      nameof(Name), null) {
      QueryHelper = queryHelper;
      Schema = TestSchema.Instance;
    }

    public Father? Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Father), value);
        _father = value;
      }
    }

    public Mother? Mother {
      get => _mother;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Mother), value);
        _mother = value;
      }
    }

    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    protected override void SetNonIdentifyingParentField(
      Type parentEntityType,
      EntityBase? newParent) {
      if (parentEntityType == typeof(Father)) {
        _father = newParent as Father;
      } else {
        _mother = newParent as Mother;
      }
    }
  }
}