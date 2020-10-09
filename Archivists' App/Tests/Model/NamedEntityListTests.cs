using System.Collections;
using System.Data;
using System.Data.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Model;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class NamedEntityListTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Data = new TestData(QueryHelper);
      Session = new TestSession();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private TestData Data { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }

    [Test]
    public void A010_Initial() {
      A010_Initial<EventTypeList>("EventType");
    }

    private void A010_Initial<TEntityList>([NotNull] string tableName)
      where TEntityList : IEntityList, new() {
      var list = new TEntityList();
      Assert.AreEqual(tableName, list.TableName, "TableName");
      Assert.IsFalse(list.IsParentList, "IsParentList");
      Assert.IsNull(list.ParentListType, "ParentListType");
      Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
      Assert.AreEqual("Name", list.Columns[0].DisplayName, "Columns[0].DisplayName");
      Assert.AreEqual(typeof(string), list.Columns[0].DataType, "Columns[0].DataType");
      list.Session = Session;
      list.Populate();
      Assert.IsEmpty(list.BindingList, "BindingList initially");
    }

    [Test]
    public void EditTable() {
      EditTable<EventTypeList>();
    }

    private void EditTable<TEntityList>()
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Interview";
      const string name3 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<NamedBindingItem>(list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name1;
      list.InsertEntityIfNew(0);
      Assert.AreEqual(1, list.Count, "Entity count after 1st add");
      var entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after 1st add");
      var item2 = editor.AddNew();
      item2.Name = name2;
      list.InsertEntityIfNew(1);
      Assert.AreEqual(2, list.Count, "Entity count after 2nd add");
      var entity2 = (INamedEntity)list[1];
      Assert.AreEqual(name2, entity2.Name, "2nd entity Name after 2nd add");
      // Refresh the grid from the saved entities on the database
      list.Populate();
      editor.SetBindingList(list.BindingList);
      Assert.AreEqual(2, editor.Count, "BindingList.Count after Populate");
      // After being refreshed by Populate, the table should now be sorted into Name order.
      Assert.AreEqual(name2, editor[0].Name, "1st item Name after populate");
      Assert.AreEqual(name1, editor[1].Name, "2nd item Name after populate");
      // Rename the first item
      editor[0].Name = name3;
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name3, entity1.Name, "1st entity Name after update");
      list.DeleteEntity(0);  // And delete it
      list.Populate(); // And refresh the grid from the database again.
      editor.SetBindingList(list.BindingList);
      Assert.AreEqual(1, list.Count, "Entity count after delete and repopulate");
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after delete and repopulate");
      Assert.AreEqual(1, editor.Count,
        "BindingList.Count after delete and repopulate");
      Assert.AreEqual(name1, editor[0].Name, "1st item Name after delete and repopulate");
    }

    [Test]
    public void ErrorOnDelete() {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(2, Session);
      Data.AddEventsPersisted(3, Session, eventType: Data.EventTypes[1]);
      Session.Commit();
      // The second EventType cannot be deleted
      // because it is a parent of 3 child Events.
      ErrorOnDelete<EventTypeList>((IList)Data.EventTypes);
    }

    private void ErrorOnDelete<TEntityList>([NotNull] IList entities)
      where TEntityList : IEntityList, new() {
      var list = new TEntityList {Session = Session};
      list.Populate(entities);
      try {
        list.DeleteEntity(1);
        Assert.Fail(
          "DeleteEntity should have thrown DatabaseUpdateErrorException.");
      } catch (DatabaseUpdateErrorException exception) {
        Assert.AreEqual(ChangeAction.Delete, exception.ChangeAction, "ChangeAction");
        Assert.IsTrue(
          exception.Message.Contains("cannot be deleted because it is referenced by"),
          "Message");
        Assert.AreEqual(1, exception.RowIndex, "RowIndex");
        Assert.IsInstanceOf(typeof(ConstraintException), exception.InnerException,
          "InnerException");
        Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
          "LastDatabaseUpdateErrorException");
      }
    }

    [Test]
    public void ErrorOnInsert() {
      ErrorOnInsert<EventTypeList>();
    }

    private void ErrorOnInsert<TEntityList>()
      where TEntityList : IEntityList, new() {
      const string name = "Performance";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<NamedBindingItem>(list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name;
      list.InsertEntityIfNew(0);
      var item2 = editor.AddNew();
      item2.Name = name;
      try {
        list.InsertEntityIfNew(1);
        Assert.Fail(
          "Duplicate name should have thrown DatabaseUpdateErrorException.");
      } catch (DatabaseUpdateErrorException exception) {
        Assert.AreEqual(ChangeAction.Insert, exception.ChangeAction, "ChangeAction");
        Assert.IsTrue(
          exception.Message.Contains(
            $"cannot be persisted because another {list.TableName} "
            + "with the same key already persists."),
          "Message");
        Assert.AreEqual(1, exception.RowIndex, "RowIndex");
        Assert.AreEqual(0, exception.ColumnIndex, "ColumnIndex");
        Assert.IsInstanceOf(typeof(DuplicateKeyException), exception.InnerException,
          "InnerException");
        Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
          "LastDatabaseUpdateErrorException");
      }
    }

    [Test]
    public void ErrorOnUpdate() {
      ErrorOnUpdate<EventTypeList>();
    }

    private void ErrorOnUpdate<TEntityList>()
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<NamedBindingItem>(list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name1;
      list.InsertEntityIfNew(0);
      var item2 = editor.AddNew();
      item2.Name = name2;
      list.InsertEntityIfNew(1);
      try {
        item2.Name = name1;
        Assert.Fail(
          "Rename should have thrown DatabaseUpdateErrorException.");
      } catch (DatabaseUpdateErrorException exception) {
        Assert.AreEqual(ChangeAction.Update, exception.ChangeAction, "ChangeAction");
        Assert.IsTrue(
          exception.Message.Contains(
            $"because another {list.TableName} "
            + "with that Name has already been persisted."),
          "Message");
        Assert.AreEqual(1, exception.RowIndex, "RowIndex");
        Assert.AreEqual(0, exception.ColumnIndex, "ColumnIndex");
        Assert.IsInstanceOf(typeof(DuplicateKeyException), exception.InnerException,
          "InnerException");
        Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
          "LastDatabaseUpdateErrorException");
      }
    }
  }
}