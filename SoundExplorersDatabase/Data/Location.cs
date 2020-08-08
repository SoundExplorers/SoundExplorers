using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Location : RelativeBase {
    private string _name;
    private string _notes;

    public Location() : base(typeof(Location)) {
      Events = new SortedChildList<Event>(this);
    }

    [NotNull] public SortedChildList<Event> Events { get; }

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
      return QueryHelper.Find<Location>(Key, session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}