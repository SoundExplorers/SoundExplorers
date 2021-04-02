using System.Data;
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
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginUpdate();
      Data.AddRootsPersistedIfRequired(Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private QueryHelper QueryHelper { get; set; } = null!;
    private TestData Data { get; set; } = null!;
    private string DatabaseFolderPath { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void A010_Initial() {
      A010_Initial<EventTypeList>("EventType");
    }

    private void A010_Initial<TEntityList>(string tableName)
      where TEntityList : IEntityList, new() {
      var list = new TEntityList();
      Assert.AreEqual(tableName, list.EntityTypeName, "EntityName");
      Assert.IsNull(list.ParentListType, "ParentListType");
      Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
      Assert.AreEqual("Name", list.Columns[0].PropertyName, "Columns[0].Name");
      list.Session = Session;
      list.Populate();
      Assert.IsEmpty(list.BindingList, "BindingList initially");
    }

    [Test]
    public void Edit() {
      Edit<EventType, EventTypeList>();
    }

    private void Edit<TEntity, TEntityList>()
      where TEntity : EntityBase, INamedEntity, new()
      where TEntityList : EntityListBase<TEntity, NamedBindingItem<TEntity>>, new() {
      const string name1 = "Performance";
      const string name2 = "Interview";
      const string name3 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var bindingList = list.BindingList;
      var item1 = bindingList.AddNew();
      list.OnRowEnter(0);
      item1.Name = name1;
      list.OnRowValidated(0);
      Assert.AreEqual(1, list.Count, "Entity count after 1st add");
      var entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after 1st add");
      var item2 = bindingList.AddNew();
      item2.Name = name2;
      list.OnRowValidated(1);
      Assert.AreEqual(2, list.Count, "Entity count after 2nd add");
      var entity2 = (INamedEntity)list[1];
      Assert.AreEqual(name2, entity2.Name, "2nd entity Name after 2nd add");
      // Refresh the grid from the saved entities on the database
      list.Populate();
      bindingList = list.BindingList;
      Assert.AreEqual(2, bindingList.Count, "editor.Count after Populate");
      // After being refreshed by Populate, the table should now be sorted into Name order.
      Assert.AreEqual(name2, bindingList[0].Name, "1st item Name after populate");
      Assert.AreEqual(name1, bindingList[1].Name, "2nd item Name after populate");
      list.OnRowValidated(0); // Should have no effect
      Assert.AreEqual(2, bindingList.Count, "editor.Count going to existing row");
      // Rename the first item
      bindingList[0].Name = name3;
      entity1 = list[0];
      Assert.AreEqual(name3, entity1.Name, "1st entity Name after update");
      list.DeleteEntity(0); // And delete it
      list.Populate(); // And refresh the grid from the database again.
      bindingList = list.BindingList;
      Assert.AreEqual(1, list.Count, "Entity count after delete and repopulate");
      entity1 = list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after delete and repopulate");
      Assert.AreEqual(1, bindingList.Count, "editor.Count after delete and repopulate");
      Assert.AreEqual(name1, bindingList[0].Name,
        "1st item Name after delete and repopulate");
    }

    [Test]
    public void ErrorOnDelete() {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(2, Session);
      Data.AddLocationsPersisted(1, Session);
      Data.AddNewslettersPersisted(1, Session);
      Data.AddSeriesPersisted(1, Session);
      Data.AddEventsPersisted(3, Session, eventType: Data.EventTypes[1]);
      Session.Commit();
      // The second EventType cannot be deleted
      // because it is a parent of 3 child Events.
      ErrorOnDelete<EventTypeList>();
    }

    private void ErrorOnDelete<TEntityList>()
      where TEntityList : IEntityList, new() {
      var list = new TEntityList {Session = Session};
      list.Populate();
      list.OnRowEnter(1);
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => list.DeleteEntity(1),
        "DeleteEntity should have thrown DatabaseUpdateErrorException.");
      Assert.AreEqual(StatementType.Delete, exception.ChangeAction, "ChangeAction");
      Assert.IsTrue(
        exception.Message.Contains("cannot be deleted because it is referenced by"),
        "Message");
      Assert.AreEqual(1, exception.RowIndex, "RowIndex");
      Assert.IsInstanceOf(typeof(ConstraintException), exception.InnerException,
        "InnerException");
      Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
        "LastDatabaseUpdateErrorException");
    }

    [Test]
    public void ErrorOnInsert() {
      ErrorOnInsert<EventType, EventTypeList>();
    }

    private void ErrorOnInsert<TEntity, TEntityList>()
      where TEntity : EntityBase, INamedEntity, new()
      where TEntityList : EntityListBase<TEntity, NamedBindingItem<TEntity>>, new() {
      const string name = "Performance";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var bindingList = list.BindingList;
      var item1 = bindingList.AddNew();
      list.OnRowEnter(0);
      item1.Name = name;
      list.OnRowValidated(0);
      var item2 = bindingList.AddNew();
      item2.Name = name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => list.OnRowValidated(1),
        "Duplicate name should have thrown DatabaseUpdateErrorException.");
      Assert.AreEqual("Another EventType with key 'Performance' already exists.",
        exception.Message, "Message");
      Assert.AreEqual(StatementType.Insert, exception.ChangeAction, "ChangeAction");
      Assert.AreEqual(1, exception.RowIndex, "RowIndex");
      Assert.AreEqual(0, exception.ColumnIndex, "ColumnIndex");
      Assert.IsInstanceOf(typeof(DuplicateNameException), exception.InnerException,
        "InnerException");
      Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
        "LastDatabaseUpdateErrorException");
    }

    [Test]
    public void ErrorOnUpdate() {
      ErrorOnUpdate<EventType, EventTypeList>();
    }

    private void ErrorOnUpdate<TEntity, TEntityList>()
      where TEntity : EntityBase, INamedEntity, new()
      where TEntityList : EntityListBase<TEntity, NamedBindingItem<TEntity>>, new() {
      const string name1 = "Performance";
      const string name2 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var bindingList = list.BindingList;
      var item1 = bindingList.AddNew();
      list.OnRowEnter(0);
      item1.Name = name1;
      list.OnRowValidated(0);
      var item2 = bindingList.AddNew();
      list.OnRowEnter(1);
      item2.Name = name2;
      list.OnRowValidated(1);
      var exception = Assert.Catch<DuplicateNameException>(
        () => item2.Name = name1,
        "Rename name should have thrown DuplicateNameException.");
      Assert.AreEqual("Another EventType with key 'Performance' already exists.",
        exception.Message, "Message");
      list.OnValidationError(1, "Name", exception);
      Assert.AreEqual(StatementType.Update,
        list.LastDatabaseUpdateErrorException!.ChangeAction, "ChangeAction");
      Assert.AreEqual(1, list.LastDatabaseUpdateErrorException!.RowIndex, "RowIndex");
      Assert.AreEqual(0, list.LastDatabaseUpdateErrorException!.ColumnIndex,
        "ColumnIndex");
      Assert.AreSame(exception, list.LastDatabaseUpdateErrorException!.InnerException,
        "InnerException");
    }

    [Test]
    public void IsInsertionRowCurrent() {
      var list = new GenreList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var bindingList = list.BindingList;
      Assert.IsFalse(list.IsInsertionRowCurrent, "IsInsertionRowCurrent initially");
      bindingList.AddNew();
      Assert.IsTrue(list.IsInsertionRowCurrent,
        "IsInsertionRowCurrent after adding insertion row");
      list.OnRowEnter(0);
      list.OnRowValidated(0);
      Assert.IsFalse(list.IsInsertionRowCurrent,
        "IsInsertionRowCurrent after cancelling insertion");
    }
  }
}