using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Series : RelativeBase {
    private string _notes;

    public Series() : base(typeof(Series), nameof(Name), null) {
      Events = new SortedChildList<Event>(this);
    }

    [NotNull] public SortedChildList<Event> Events { get; }

    [CanBeNull]
    public string Name {
      get => SimpleKey;
      set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

    [CanBeNull]
    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}