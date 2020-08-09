using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Set : RelativeBase {
    private Act _act;
    private Event _event;
    private string _notes;
    private int _setNo;

    public Set() : base(typeof(Set)) { }

    [CanBeNull]
    public Act Act {
      get => _act;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Act), value);
        _act = value;
      }
    }

    [NotNull]
    public Event Event {
      get => _event;
      set {
        UpdateNonIndexField();
        ChangeParent(typeof(Event), value);
        _event = value;
      }
    }

    public override RelativeBase IdentifyingParent => Event;

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
        SetKey(value.ToString().PadLeft(2, '0'));
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

    protected override void OnParentFieldToBeUpdated(Type parentPersistableType,
      RelativeBase newParent) {
      if (parentPersistableType == typeof(Event)) {
        _event = (Event)newParent;
      } else {
        _act = (Act)newParent;
      }
    }
  }
}