using System;
using System.Collections;
using System.Data.Linq;
using JetBrains.Annotations;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Data {
  public class Newsletter : RelativeBase {
    private DateTime _date;
    private string _path;

    public Newsletter() : base(typeof(Newsletter)) {
      Events = new SortedChildList<string, Event>(this);
    }

    public DateTime Date {
      get => _date;
      set {
        UpdateNonIndexField();
        _date = value;
        SetKey(value);
      }
    }

    [NotNull] public SortedChildList<string, Event> Events { get; }

    public string Path {
      get => _path;
      set {
        UpdateNonIndexField();
        _path = value;
      }
    }

    protected override void CheckCanPersist(SessionBase session) {
      var pathDuplicate = FindPathDuplicate(session);
      if (pathDuplicate != null) {
        throw new DuplicateKeyException(
          this,
          $"Newsletter '{Date:yyyy/MM/dd}' " +
          "cannot be persisted because Newsletter " +
          $"'{pathDuplicate.Date:yyyy/MM/dd}' "
          + $"already persists with the same path '{Path}'.");
      }
      base.CheckCanPersist(session);
    }

    private Newsletter FindPathDuplicate([NotNull] SessionBase session) {
      return QueryHelper.Find<Newsletter>(
        newsletter => newsletter.Path == Path, session);
    }

    protected override RelativeBase FindWithSameKey(SessionBase session) {
      return QueryHelper.Find<Newsletter>(Key, session);
    }

    protected override IDictionary GetChildren(Type childType) {
      return Events;
    }

    protected override void OnParentFieldToBeUpdated(
      Type parentPersistableType, RelativeBase newParent) {
      throw new NotSupportedException();
    }
  }
}