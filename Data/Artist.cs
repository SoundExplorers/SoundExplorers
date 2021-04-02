using System;
using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.Data {
  /// <summary>
  ///   Artist entity, usually representing a musician.
  /// </summary>
  public class Artist : EntityBase {
    private string _forename = null!;
    private string _notes = null!;
    private string _surname = null!;

    [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
    public Artist() : base(typeof(Artist), nameof(Name), null) {
      Credits = new SortedEntityCollection<Credit>();
    }

    public SortedEntityCollection<Credit> Credits { get; }

    /// <summary>
    ///   The Name of an Artist who goes by a single name
    ///   may be specified as either Forename or Surname.
    /// </summary>
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
    public string Name {
      get => SimpleKey;
      private set {
        UpdateNonIndexField();
        SimpleKey = value;
      }
    }

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
    public string Surname {
      get => _surname;
      set {
        UpdateNonIndexField();
        _surname = value;
        Name = MakeName(Forename, value);
      }
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Credits;
    }

    /// <summary>
    ///   Combines forename and surname into a single name,
    ///   allowing for artists who go by a single name.
    /// </summary>
    public static string MakeName(string? forename,
      string? surname) {
      string result;
      if (!string.IsNullOrWhiteSpace(forename)) {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!string.IsNullOrWhiteSpace(surname)) {
          result = $"{surname.Trim()}, {forename.Trim()}";
        } else {
          result = forename.Trim();
        }
      } else if (!string.IsNullOrWhiteSpace(surname)) {
        result = surname.Trim();
      } else {
        result = string.Empty;
      }
      return result;
    }
  }
}