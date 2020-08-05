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
        SetKey($"{value:yyyy/MM/dd}");
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
      base.CheckCanPersist(session);
      var pathDuplicate = FindPathDuplicate(session);
      if (pathDuplicate != null) {
        throw new DuplicateKeyException(
          this,
          $"Newsletter '{Key}' " +
          "cannot be persisted because Newsletter " +
          $"'{pathDuplicate.Key}' "
          + $"already persists with the same path '{Path}'.");
      }
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