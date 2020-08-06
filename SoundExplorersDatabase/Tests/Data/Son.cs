using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Son : RelativeBase {
    private Father _father;
    private Mother _mother;
    private string _name;

    public Son([NotNull] TestSchema schema) : base(typeof(Son)) {
      Schema = schema ?? throw new ArgumentNullException(nameof(schema));
    }

    public Father Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Father), value);
        _father = value;
      }
    }

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
        SetKey(value);
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