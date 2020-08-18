using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class Artist : RelativeBase {
    private string _forename;
    private string _notes;
    private string _surname;

    public Artist() : base(typeof(Artist), nameof(Name), null) {
      //Credits = new SortedChildList<Credit>(this);
    }

    public string Forename {
      get => _forename;
      set {
        UpdateNonIndexField();
        _forename = value;
        UpdateName(value, Surname);
      }
    }

    public string Name {
      get => SimpleKey;
      private set => SimpleKey = value;
    }

    public string Notes {
      get => _notes;
      set {
        UpdateNonIndexField();
        _notes = value;
      }
    }

    public string Surname {
      get => _surname;
      set {
        UpdateNonIndexField();
        _surname = value;
        UpdateName(Forename, value);
      }
    }

    //[NotNull] public SortedChildList<Credit> Credits { get; }

    protected override IDictionary GetChildren(Type childType) {
      //return Credits;
      throw new NotImplementedException();
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }

    private void UpdateName([CanBeNull] string forename,
      [CanBeNull] string surname) {
      Name = ((forename?.Trim() ?? string.Empty) + " " +
              (surname ?? string.Empty)).Trim();
    }
  }
}