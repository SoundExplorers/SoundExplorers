using System;
using System.Collections;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorers.Data {
  /// <summary>
  ///   An entity representing a newsletter that documents one or more Events.
  /// </summary>
  public class Newsletter : EntityBase {
    private DateTime _date;
    private string _url;

    public Newsletter() : base(typeof(Newsletter), nameof(Date), null) {
      _date = InitialDate;
      Events = new SortedChildList<Event>();
    }

    /// <summary>
    ///   The newsletter's publication date (email send date).
    /// </summary>
    /// <exception cref="PropertyConstraintException"></exception>
    public DateTime Date {
      get => _date;
      set {
        if (value <= InitialDate) {
          throw new PropertyConstraintException(
            $"Newsletter Date must be later than {DateToSimpleKey(InitialDate)}.",
            nameof(Date));
        }
        UpdateNonIndexField();
        _date = value.Date;
        SimpleKey = DateToSimpleKey(_date);
      }
    }

    [NotNull] public SortedChildList<Event> Events { get; }

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

    private void CheckCanChangeUrl([CanBeNull] string oldUrl,
      [CanBeNull] string newUrl) {
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

    [CanBeNull]
    private Newsletter FindDuplicateUrl([NotNull] string url,
      [NotNull] SessionBase session) {
      return QueryHelper.Find<Newsletter>(
        newsletter => newsletter.Url.Equals(url) && !newsletter.Oid.Equals(Oid),
        session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void SetNonIdentifyingParentField(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}