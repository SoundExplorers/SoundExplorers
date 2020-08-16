﻿using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Son : RelativeBase {
    private Father _father;
    private Mother _mother;

    public Son([NotNull] QueryHelper queryHelper) : base(typeof(Son),
      nameof(Name), null) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
    }

    public Father Father {
      get => _father;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Father), value);
        _father = value;
      }
    }

    public Mother Mother {
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

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey(SessionBase session) {
      throw new NotSupportedException();
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary GetChildren(Type childType) {
      throw new NotSupportedException();
    }

    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Father)) {
        _father = (Father) newParent;
      }
      else {
        _mother = (Mother) newParent;
      }
    }
  }
}