using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Act : KeyedRelative {
    private string _name;
    private string _notes;

    public Act() : base(typeof(Act), nameof(Name)) {
      Sets = new SortedChildList<Set>(this);
    }

    [NotNull] public SortedChildList<Set> Sets { get; }

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
      return QueryHelper.Find<Act>(GetSimpleKey(), session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Sets;
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