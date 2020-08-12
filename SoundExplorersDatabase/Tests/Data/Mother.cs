using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  public class Mother : KeyedRelative {
    private string _name;

    public Mother([NotNull] QueryHelper queryHelper) : base(typeof(Mother), nameof(Name)) {
      QueryHelper = queryHelper ??
                    throw new ArgumentNullException(nameof(queryHelper));
      Schema = TestSchema.Instance;
      Daughters = new SortedChildList<Daughter>(this);
      Sons = new SortedChildList<Son>(this);
    }

    [NotNull] public SortedChildList<Daughter> Daughters { get; }

    [NotNull]
    public string Name {
      get => _name;
      set {
        UpdateNonIndexField();
        _name = value;
      }
    }

    [NotNull] public SortedChildList<Son> Sons { get; }

    protected override KeyedRelative FindWithSameKey(
      SessionBase session) {
      return QueryHelper.Find<Mother>(SimpleKey, session);
    }

    protected override IDictionary GetChildren(Type childType) {
      if (childType == typeof(Daughter)) {
        return Daughters;
      }
      return Sons;
    }

    protected override KeyedRelative GetIdentifyingParent() {
      return null;
    }

    protected override string GetSimpleKey() {
      return Name;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType,
      KeyedRelative newParent) {
      throw new NotSupportedException();
    }
  }
}