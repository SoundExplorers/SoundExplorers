using System;
using System.Linq;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a newsletter that documents one or more Events.
  /// </summary>
  public class Newsletter : EntityBase {
    private DateTime _date;
    private string _url = null!;

    public Newsletter() : base(typeof(Newsletter), nameof(Date), null) {
      _date = DefaultDate;
      Events = new SortedEntityCollection<Event>();
    }

    /// <summary>
    ///   The newsletter's publication date (email send date).
    /// </summary>
    /// <exception cref="PropertyConstraintException"></exception>
    public DateTime Date {
      get => _date;
      set {
        if (value < DefaultDate) {
          throw new PropertyConstraintException(
            "Newsletter Date must be later than or equal to " +
            $"{DateToSimpleKey(DefaultDate)}. ({DateToSimpleKey(DefaultDate)} " +
            "indicates that an Event's Newsletter has not yet been specified.)",
            nameof(Date));
        }
        UpdateNonIndexField();
        _date = value.Date;
        SimpleKey = DateToSimpleKey(_date);
      }
    }

    public SortedEntityCollection<Event> Events { get; }

    /// <summary>
    ///   The URL where the newsletter is archived.
    ///   Must be specified and unique.
    /// </summary>
    public string Url {
      get => _url;
      set {
        CheckCanChangeUrl(_url, value);
        UpdateNonIndexField();
        _url = value;
      }
    }

    private void CheckCanChangeUrl(string? oldUrl,
      string? newUrl) {
      ValidateUrlFormatOnUpdate(newUrl, Date);
      if (IsPersistent && Session != null && newUrl != oldUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateUrl(newUrl!);
        if (duplicate != null) {
          throw CreateDuplicateUrlUpdateException(duplicate);
        }
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      ValidateUrlFormatOnInsertion(Url);
      var urlDuplicate = FindDuplicateUrl(Url);
      if (urlDuplicate != null) {
        throw CreateDuplicateUrlInsertionException(Url, Date, urlDuplicate);
      }
    }

    /// <summary>
    ///   Creates a dummy Newsletter, to be the default for Events for which a Newsletter
    ///   has not yet been specified.
    /// </summary>
    public static Newsletter CreateDefault() {
      return new Newsletter {
        Date = DefaultDate,
        Url = "Required default"
      };
    }

    public static PropertyConstraintException CreateDuplicateUrlInsertionException(
      string url, DateTime date, Newsletter duplicate) {
      return new PropertyConstraintException(
        $"Newsletter {DateToSimpleKey(date)} cannot be added because Newsletter " +
        $"{duplicate.SimpleKey} " +
        $"already exists with the same URL '{url}'.", nameof(Url));
    }

    public static PropertyConstraintException CreateDuplicateUrlUpdateException(
      Newsletter duplicate) {
      return new PropertyConstraintException(
        "URL cannot be set to " +
        $"'{duplicate.Url}'. Newsletter {duplicate.SimpleKey} " +
        "already exists with that URL.", nameof(Url));
    }

    private Newsletter? FindDuplicateUrl(string url) {
      var newsletters = 
        (Root as SortedEntityCollection<Newsletter>)!.Values;
      // var newsletters = session.AllObjects<Newsletter>().ToList();
      return (
        from newsletter in newsletters
        where newsletter.Url == url && !newsletter.Oid.Equals(Oid)
        select newsletter).FirstOrDefault();
    }

    protected override ISortedEntityCollection GetChildren(Type childType) {
      return Events;
    }

    public static void ValidateUrlFormatOnInsertion(string url) {
      if (string.IsNullOrWhiteSpace(url)) {
        throw new PropertyConstraintException(
          "Newsletter cannot be added because a URL has not been specified.",
          nameof(Url));
      }
    }

    public static void ValidateUrlFormatOnUpdate(string? url, DateTime date) {
      if (string.IsNullOrWhiteSpace(url)) {
        throw new PropertyConstraintException(
          "A valid URL has not been specified for Newsletter " +
          $"{DateToSimpleKey(date)}.",
          nameof(Url));
      }
      if (date == DefaultDate) {
        // The default Newsletter is an automatically created dummy, used as the default
        // for Events for which a Newsletter has not yet been specified. So it just has a
        // "Required default" comment in its URL property.
        return;
      }
      ValidateUrlFormat(url, nameof(Url));
    }
  }
}