using System;
using System.ComponentModel;
using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model {
  public class TestEditor<TEntity, TBindingItem>
    where TEntity : EntityBase, new()
    where TBindingItem : BindingItemBase<TEntity, TBindingItem>, new() {
    public TestEditor([NotNull] QueryHelper queryHelper, [NotNull] SessionBase session,
      IBindingList bindingList = null) {
      QueryHelper = queryHelper;
      Session = session;
      SetBindingList(bindingList);
    }

    private BindingList<TBindingItem> BindingList { get; set; }

    [NotNull]
    public TBindingItem this[int index] {
      get {
        var result = BindingList[index];
        result.QueryHelper = QueryHelper;
        result.Session = Session;
        return result;
      }
    }

    [NotNull] private QueryHelper QueryHelper { get; }
    [NotNull] private SessionBase Session { get; }
    public int Count => BindingList.Count;

    /// <summary>
    ///   Emulates adding a new item to the grid via the insertion row.
    /// </summary>
    [NotNull]
    public TBindingItem AddNew() {
      var result = BindingList.AddNew() ?? throw new NullReferenceException();
      result.QueryHelper = QueryHelper;
      result.Session = Session;
      return result;
    }

    public void SetBindingList(IBindingList bindingList) {
      BindingList = (BindingList<TBindingItem>)bindingList;
    }
  }
}