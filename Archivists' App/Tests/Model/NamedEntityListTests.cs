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
      var list = new TEntityList {IsParentList = true};
      Assert.AreEqual(tableName, list.EntityName, "EntityName");
      Assert.IsTrue(list.IsParentList, "IsParentList");
      Assert.IsNull(list.ParentListType, "ParentListType");
      Assert.AreEqual(1, list.Columns.Count, "Columns.Count");
      Assert.AreEqual("Name", list.Columns[0].Name, "Columns[0].Name");
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
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Interview";
      const string name3 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<TEntity, NamedBindingItem<TEntity>>(
        list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name1;
      list.OnRowValidated(0);
      Assert.AreEqual(1, list.Count, "Entity count after 1st add");
      var entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after 1st add");
      var item2 = editor.AddNew();
      item2.Name = name2;
      list.OnRowValidated(1);
      Assert.AreEqual(2, list.Count, "Entity count after 2nd add");
      var entity2 = (INamedEntity)list[1];
      Assert.AreEqual(name2, entity2.Name, "2nd entity Name after 2nd add");
      // Refresh the grid from the saved entities on the database
      list.Populate();
      editor.SetBindingList(list.BindingList);
      Assert.AreEqual(2, editor.Count, "editor.Count after Populate");
      // After being refreshed by Populate, the table should now be sorted into Name order.
      Assert.AreEqual(name2, editor[0].Name, "1st item Name after populate");
      Assert.AreEqual(name1, editor[1].Name, "2nd item Name after populate");
      list.OnRowValidated(0); // Should have no effect
      Assert.AreEqual(2, editor.Count, "editor.Count going to existing row");
      // Rename the first item
      editor[0].Name = name3;
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name3, entity1.Name, "1st entity Name after update");
      list.DeleteEntity(0); // And delete it
      list.Populate(); // And refresh the grid from the database again.
      editor.SetBindingList(list.BindingList);
      Assert.AreEqual(1, list.Count, "Entity count after delete and repopulate");
      entity1 = (INamedEntity)list[0];
      Assert.AreEqual(name1, entity1.Name, "1st entity Name after delete and repopulate");
      Assert.AreEqual(1, editor.Count, "editor.Count after delete and repopulate");
      Assert.AreEqual(name1, editor[0].Name, "1st item Name after delete and repopulate");
    }

    [Test]
    public void ErrorOnDelete() {
      Session.BeginUpdate();
      Data.AddEventTypesPersisted(2, Session);
      Data.AddLocationsPersisted(1, Session);
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
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => list.DeleteEntity(1),
        "DeleteEntity should have thrown DatabaseUpdateErrorException.");
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

    [Test]
    public void ErrorOnInsert() {
      ErrorOnInsert<EventType, EventTypeList>();
    }

    private void ErrorOnInsert<TEntity, TEntityList>()
      where TEntity : EntityBase, INamedEntity, new()
      where TEntityList : IEntityList, new() {
      const string name = "Performance";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<TEntity, NamedBindingItem<TEntity>>(
        list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name;
      list.OnRowValidated(0);
      var item2 = editor.AddNew();
      item2.Name = name;
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => list.OnRowValidated(1),
        "Duplicate name should have thrown DatabaseUpdateErrorException.");
      Assert.AreEqual(ChangeAction.Insert, exception.ChangeAction, "ChangeAction");
      Assert.IsTrue(
        exception.Message.Contains(
          $"cannot be added because another {list.EntityName} "
          + "with the same key already persists."),
        "Message");
      Assert.AreEqual(1, exception.RowIndex, "RowIndex");
      Assert.AreEqual(0, exception.ColumnIndex, "ColumnIndex");
      Assert.IsInstanceOf(typeof(PropertyConstraintException), exception.InnerException,
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
      where TEntityList : IEntityList, new() {
      const string name1 = "Performance";
      const string name2 = "Rehearsal";
      var list = new TEntityList {Session = Session};
      list.Populate(); // Creates an empty BindingList
      var editor = new TestEditor<TEntity, NamedBindingItem<TEntity>>(
        list.BindingList);
      var item1 = editor.AddNew();
      item1.Name = name1;
      list.OnRowValidated(0);
      var item2 = editor.AddNew();
      item2.Name = name2;
      list.OnRowValidated(1);
      var exception = Assert.Catch<DatabaseUpdateErrorException>(
        () => item2.Name = name1,
        "Rename name should have thrown DatabaseUpdateErrorException.");
      Assert.AreEqual(ChangeAction.Update, exception.ChangeAction, "ChangeAction");
      Assert.IsTrue(
        exception.Message.Contains(
          $"because another {list.EntityName} "
          + "with that Name already exists."),
        "Message");
      Assert.AreEqual(1, exception.RowIndex, "RowIndex");
      Assert.AreEqual(0, exception.ColumnIndex, "ColumnIndex");
      Assert.IsInstanceOf(typeof(PropertyConstraintException), exception.InnerException,
        "InnerException");
      Assert.AreSame(exception, list.LastDatabaseUpdateErrorException,
        "LastDatabaseUpdateErrorException");
    }
  }
}