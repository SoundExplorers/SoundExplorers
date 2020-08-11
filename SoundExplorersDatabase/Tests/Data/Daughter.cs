using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Daughter : RelativeBase {
    private Father _father;
    private Mother _mother;
    private string _name;

    public Daughter([NotNull] QueryHelper queryHelper) : base(typeof(Daughter), nameof(Name)) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
    }

    public Father Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Father), value);
        _father = value;
      }
    }

    [NotNull]
    public Mother Mother {
      get => _mother;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Mother), value);
        _mother = value;
      }
    }

    [NotNull]
    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey(SessionBase session) {
      throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    protected override RelativeBase GetIdentifyingParent() {
      return null;
    }

    protected override string GetSimpleKey() {
      return Name;
    }

    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Father)) {
        _father = (Father)newParent;
      } else {
        _mother = (Mother)newParent;
      }
    }
  }
}