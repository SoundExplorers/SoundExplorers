using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Location : KeyedRelative {
    private string _name;
    private string _notes;

    public Location() : base(typeof(Location), nameof(Name)) {
      Events = new SortedChildList<Event>(this);
    }

    [NotNull] public SortedChildList<Event> Events { get; }

    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
      }
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override KeyedRelative FindWithSameKey(SessionBase session) {
      return QueryHelper.Find<Location>(GetSimpleKey(), session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    protected override KeyedRelative GetIdentifyingParent() {
      return null;
    }

    protected override string GetSimpleKey() {
      return Name;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType, KeyedRelative newParent) {
      throw new NotSupportedException();
    }
  }
}