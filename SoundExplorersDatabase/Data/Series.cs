using System;
using System.Collections;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Series : RelativeBase {
    private string _name;
    private string _notes;

    public Series() : base(typeof(Series)) {
      Events = new SortedChildList<string, Event>(this);
    }

    [NotNull] public SortedChildList<string, Event> Events { get; }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
        SetKey(value);
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override RelativeBase FindWithSameKey(SessionBase session) {
      return QueryHelper.Find<Series>(Key, session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}