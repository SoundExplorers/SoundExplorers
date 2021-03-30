using System;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  public class Daughter : EntityBase {
    private Father? _father;
    private Mother? _mother;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Daughter(SortedEntityCollection<Daughter> root, QueryHelper queryHelper,
      Type? identifyingParentType = null) : base(root,typeof(Daughter),
      nameof(Name), identifyingParentType) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
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

    public virtual Mother? Mother {
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