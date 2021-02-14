using System;
using System.Collections;
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
      Events = new SortedChildList<Event>();
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

    public SortedChildList<Event> Events { get; }

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
      if (string.IsNullOrWhiteSpace(newUrl)) {
        throw new PropertyConstraintException(
          $"A valid URL has not been specified for Newsletter '{SimpleKey}'.",
          nameof(Url));
      }
      try {
        var dummy = new Uri(newUrl, UriKind.Absolute);
      } catch (UriFormatException) {
        throw new PropertyConstraintException(
          $"Invalid URL format: '{newUrl}'.", nameof(Url));
      }
      if (IsPersistent && Session != null && newUrl != oldUrl) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateUrl(newUrl, Session);
        if (duplicate != null) {
          throw new PropertyConstraintException(
            "URL cannot be set to " +
            $"'{newUrl}'. Newsletter {duplicate.SimpleKey} " +
            "already exists with that URL.", nameof(Url));
        }
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      if (string.IsNullOrWhiteSpace(Url)) {
        if (Date == DefaultDate) {
          return;
        }
        throw new PropertyConstraintException(
          "Newsletter cannot be added because a URL has not been specified.",
          nameof(Url));
      }
      var urlDuplicate = FindDuplicateUrl(Url, session);
      if (urlDuplicate != null) {
        throw new PropertyConstraintException(
          "Newsletter cannot be added because Newsletter " +
          $"'{urlDuplicate.SimpleKey}' " +
          $"already exists with the same URL '{Url}'.", nameof(Url));
      }
    }

    private Newsletter? FindDuplicateUrl(string url,
      SessionBase session) {
      var newsletters = session.AllObjects<Newsletter>().ToList();
      return (
        from newsletter in newsletters
        where newsletter.Url == url && !newsletter.Oid.Equals(Oid)
        select newsletter).FirstOrDefault();
      // NewsletterTests fail with this, for unknown reason.
      // The predicate makes it throw a NullReferenceException.
      // return QueryHelper.Find<Newsletter>(
      //   newsletter => newsletter.Url.Equals(url) && !newsletter.Oid.Equals(Oid),
      //   session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }
  }
}