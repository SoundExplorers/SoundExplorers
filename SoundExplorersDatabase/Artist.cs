using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SoundExplorersDatabase {
  /// <summary>
  ///   Artist entity, usually representing a musician.
  /// </summary>
  public class Artist : EntityBase {
    private string _forename;
    private string _notes;
    private string _surname;

    public Artist() : base(typeof(Artist), nameof(Name), null) {
      Credits = new SortedChildList<Credit>(this);
    }

    [NotNull] public SortedChildList<Credit> Credits { get; }

    /// <summary>
    ///   The Name of an Artist who goes by a single name
    ///   may be specified as either Forename or Surname.
    /// </summary>
    [CanBeNull]
    public string Forename {
      get => _forename;
      set {
        UpdateNonIndexField();
        _forename = value;
        Name = MakeName(value, Surname);
      }
    }

    /// <summary>
    ///   Combines Forename and Surname into a single name.
    ///   So the name of an Artist who goes by a single name
    ///   may be specified as either Forename or Surname.
    /// </summary>
    [CanBeNull]
    public string Name {
      get => SimpleKey;
      private set {
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

    /// <summary>
    ///   The Name of an Artist who goes by a single name
    ///   may be specified as either Forename or Surname.
    /// </summary>
    [CanBeNull]
    public string Surname {
      get => _surname;
      set {
        UpdateNonIndexField();
        _surname = value;
        Name = MakeName(Forename, value);
      }
    }

    protected override IDictionary GetChildren(Type childType) {
      return Credits;
    }

    /// <summary>
    ///   Combines forename and surname into a single name,
    ///   allowing for artists who go by a single name.
    /// </summary>
    [CanBeNull]
    private static string MakeName([CanBeNull] string forename,
      [CanBeNull] string surname) {
      string result;
      if (forename != null) {
        if (surname != null) {
          result = forename.Trim() + " " + surname.Trim();
        } else {
          result = forename.Trim();
        }
      } else if (surname != null) {
        result = surname.Trim();
      } else {
        // A NoNullAllowedException will be thrown when Name is set to null.
        // But here it is for completeness!
        result = null;
      }
      return result;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}