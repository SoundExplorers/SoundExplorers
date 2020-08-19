using System;
using System.Collections;
using System.Data;
using System.Data.Linq;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Newsletter : EntityBase {
    private DateTime _date;
    private Uri _url;

    public Newsletter() : base(typeof(Newsletter), nameof(Date), null) {
      Events = new SortedChildList<Event>(this);
    }

    public DateTime Date {
      get => _date;
      set {
        CheckCanSetDate(value);
        UpdateNonIndexField();
        _date = value;
        SimpleKey = $"{Date:yyyy/MM/dd}";
      }
    }

    [NotNull] public SortedChildList<Event> Events { get; }

    public Uri Url {
      get => _url;
      set {
        CheckCanSetUrl(value);
        UpdateNonIndexField();
        _url = value;
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      base.CheckCanPersist(session);
      if (Url == null) {
        throw new NoNullAllowedException(
          $"Newsletter '{SimpleKey}' " +
          "cannot be persisted because a URL has not been specified.");
      }
      var urlDuplicate = FindDuplicateUrl(Url, session);
      if (urlDuplicate != null) {
        throw new DuplicateKeyException(
          this,
          $"Newsletter '{SimpleKey}' " +
          "cannot be persisted because Newsletter " +
          $"'{urlDuplicate.SimpleKey}' " +
          $"has already been persisted with the same URL '{Url}'.");
      }
    }

    private void CheckCanSetDate(DateTime date) {
      if (date == DateTime.MinValue) {
        throw new NoNullAllowedException(
          "A valid newsletter date has not been specified.");
      }
      if (IsPersistent && date != _date && Session != null) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateDate(date, Session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"Newsletter {SimpleKey}'s date cannot be changed to " +
            $"{duplicate.SimpleKey} because another newsletter " +
            $"with the that date has already been persisted.");
        }
      }
    }

    private void CheckCanSetUrl([CanBeNull] Uri url) {
      if (url == null) {
        throw new NoNullAllowedException(
          $"A valid URL has not been specified for Newsletter {SimpleKey}.");
      }
      if (IsPersistent && url != _url && Session != null) {
        // If there's no session, which means we cannot check for a duplicate,
        // EntityBase.UpdateNonIndexField will throw 
        // an InvalidOperationException anyway.
        var duplicate = FindDuplicateUrl(url, Session);
        if (duplicate != null) {
          throw new DuplicateKeyException(
            this,
            $"Newsletter {SimpleKey}'s URL cannot be changed to " +
            $"{url}. Newsletter {duplicate.SimpleKey} " +
            $"has already been persisted with the that URL.");
        }
      }
    }

    [CanBeNull]
    private Newsletter FindDuplicateDate(DateTime date, [NotNull] SessionBase session) {
      return QueryHelper.Find<Newsletter>(newsletter => newsletter.Date == date,
        session);
    }

    [CanBeNull]
    private Newsletter FindDuplicateUrl([NotNull] Uri url, [NotNull] SessionBase session) {
      return QueryHelper.Find<Newsletter>(
        newsletter => newsletter.Url.Equals(Url),
        session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    [ExcludeFromCodeCoverage]
    protected override void OnNonIdentifyingParentFieldToBeUpdated(
      Type parentEntityType, EntityBase newParent) {
      throw new NotSupportedException();
    }
  }
}