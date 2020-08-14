using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Set : RelativeBase {
    private Act _act;
    private string _notes;
    private int _setNo;

    public Set() : base(typeof(Set), nameof(SetNo), typeof(Event)) { }

    [CanBeNull]
    public Act Act {
      get => _act;
      set {
        UpdateNonIndexField();
        ChangeNonIdentifyingParent(typeof(Act), value);
        _act = value;
      }
    }

    [NotNull]
    public Event Event {
      get => (Event)IdentifyingParent;
      set {
        UpdateNonIndexField();
        IdentifyingParent = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public int SetNo {
      get => _setNo;
      set {
        UpdateNonIndexField();
        _setNo = value;
        SimpleKey = value.ToString().PadLeft(2, '0');
      }
    }

    [ExcludeFromCodeCoverage]
    protected override RelativeBase FindWithSameKey(
      [NotNull] SessionBase session) {
      throw new NotSupportedException();
    }

    protected override IDictionary GetChildren(Type childType) {
      throw new NotImplementedException();
    }

    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      _act = (Act)newParent;
    }
  }
}