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
      var item1 = (NamedBindingItem)list.BindingList.AddNew();
      Assert.IsNotNull(item1, "item1");
      item1.Name = name1;
      list.InsertEntityIfNew(0);
      Assert.AreEqual(1, list.Count, "Entity count after 1st add");
      var entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after 1st add");
      var item2 = (NamedBindingItem)list.BindingList.AddNew();
      Assert.IsNotNull(item2, "item2");
      item2.Name = name2;
      list.InsertEntityIfNew(1);
      Assert.AreEqual(2, list.Count, "Entity count after 2nd add");
      var entity2 = (INamedEntity)list[1];
      Assert.AreEqual(name2, entity2.Name, "2nd entity Name after 2nd add");
      list.Populate();
      Assert.AreEqual(2, list.BindingList.Count, "BindingList.Count after Populate");
      // After being refreshed by Populate, the table should now be sorted into Name order.
      item1 = (NamedBindingItem)list.BindingList[0];
      item2 = (NamedBindingItem)list.BindingList[1];
      Assert.AreEqual(name2, item1.Name, "1st item Name after populate");
      Assert.AreEqual(name1, item2.Name, "2nd item Name after populate");
      item1.Name = name3;
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name3, entity1.Name, "1st entity Name after update");
      list.DeleteEntityIfFound(0);
      list.Populate();
      Assert.AreEqual(1, list.Count, "Entity count after delete and repopulate");
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after delete and repopulate");
      Assert.AreEqual(1, list.BindingList.Count,
        "BindingList.Count after delete and repopulate");
      item1 = (NamedBindingItem)list.BindingList[0];
      Assert.AreEqual(name1, item1.Name, "1st item Name after delete and repopulate");
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
      list.BindingList.AddNew(); // Grid's insertion row
      // When the data has been fully loaded on Populate,
      // the grid's insertion row is automatically entered.
      // Due to an eccentricity of the data load
      // (see comment in EntityListBase.DeleteEntityIfFound),
      // this needs to to be known to have occured before
      // deletions can be done.
      list.OnRowEnter(2);
      try {
        list.DeleteEntityIfFound(1);
        Assert.Fail(
          "DeleteEntityIfFound should have thrown DatabaseUpdateErrorException.");
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
    public void ErrorOnUpdate() {
      ErrorOnUpdate<EventTypeList>();
    }

    private void ErrorOnUpdate<TEntityList>()
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var item1 = (NamedBindingItem)list.BindingList.AddNew();
      Assert.IsNotNull(item1, "item1");
      item1.Name = name1;
      list.InsertEntityIfNew(0);
      var item2 = (NamedBindingItem)list.BindingList.AddNew();
      Assert.IsNotNull(item2, "item2");
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
            + "with the that Name has already been persisted."),
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