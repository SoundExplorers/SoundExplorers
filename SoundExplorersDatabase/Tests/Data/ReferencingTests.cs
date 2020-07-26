using System;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class ReferencingTests {
    [SetUp]
    public void Setup() {
      Parent1 = new Parent();
      Parent2 = new Parent {Name = Parent2Name};
      Child1 = new Child();
      Child2 = new Child();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Parent1);
        Parent1.Name = Parent1Name;
        session.Persist(Parent2);
        session.Persist(Child1);
        Child1.Name = Child1Name;
        session.Persist(Child2);
        Child2.Name = Child2Name;
        Assert.IsTrue(Parent1.Children.Add(Child1), "Added Child1 to Parent1");
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Child1Name = "Alison";
    private const string Child2Name = "Bertha the Cool";
    private const string Parent1Name = "Adrienne";
    private const string Parent2Name = "Bob";

    private Child Child1 { get; set; }
    private Child Child2 { get; set; }
    private string DatabaseFolderPath { get; set; }
    private Parent Parent1 { get; set; }
    private Parent Parent2 { get; set; }

    [Test]
    public void T10_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Parent1 = Parent.Read(Parent1Name, session);
        Parent2 = Parent.Read(Parent2Name, session);
        Child1 = Child.Read(Child1Name, session);
        Child2 = Child.Read(Child2Name, session);
        session.Commit();
        Assert.IsTrue(Child1.IsPersistent, "Child1.IsPersistent initially");
        Assert.AreEqual(Child1Name, Child1.Name, "Child1.Name");
        Assert.IsTrue(Parent1.IsPersistent, "Parent1.IsPersistent initially");
        Assert.AreEqual(Parent1Name, Parent1.Name, "Parent1.Name initially");
        Assert.AreEqual(1, Parent1.Children.Count, "Parent1.Children.Count initially");
        Assert.AreEqual(1, Parent1.References.Count,
          "Parent1.References.Count after Add #1");
        Assert.AreEqual(1, Parent1.Children.Count, "Parent1.Children.Count initially");
        Assert.AreEqual(1, Parent1.References.Count,
          "Parent1.References.Count initially");
        Assert.AreSame(Child1, Parent1.Children[0], "Parent1 1st child initially");
        Assert.AreSame(Child1, Parent1.Children[Child1Name],
          "1st child by name initially");
        Assert.AreSame(Parent1, Child1.Parent, "Child1.Parent initially");
        Assert.AreEqual(Parent2Name, Parent2.Name, "Parent2.Name initially");
        Assert.AreEqual(0, Parent2.Children.Count, "Parent2.Children.Count initially");
        Assert.AreEqual(0, Parent2.References.Count,
          "Parent2.References.Count initially");
        Assert.IsNull(Child2.Parent, "Child2.Parent initially");
      }
    }

    [Test]
    public void T20_ParentChildrenUnsupportedMethods() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent1 = Parent.Read(Parent1Name, session);
        Child2 = Child.Read(Child2Name, session);
        Assert.Throws<NotSupportedException>(
          () => Parent1.Children.Add(Child2Name, Child2),
          "Unsupported Parent.Children.Add");
        Assert.Throws<NotSupportedException>(
          () => Parent1.Children.Remove(Child1Name),
          "Unsupported Parent.Children.Remove");
        Assert.Throws<NotSupportedException>(
          () => Parent1.Children.RemoveAt(0),
          "Unsupported Parent.Children.RemoveAt");
        session.Commit();
      }
    }

    [Test]
    public void T30_CannotDeleteParentWithChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() => Parent1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void T40_AddRemoveChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent1 = Parent.Read(Parent1Name, session);
        Child1 = Child.Read(Child1Name, session);
        Child2 = Child.Read(Child2Name, session);
        Assert.IsTrue(Parent1.Children.Add(Child2), "Added Child2 to Parent1");
        session.Commit();
        Assert.AreSame(Parent1, Child2.Parent, "Child2.Parent after Add #2");
        Assert.AreSame(Child2, Parent1.Children[1], "2nd child after Add #2");
        Assert.AreEqual(2, Parent1.Children.Count, "Parent1.Children.Count after Add #2");
        Assert.AreEqual(2, Parent1.References.Count,
          "Parent1.References.Count after Add #2");
        session.BeginUpdate();
        Assert.IsTrue(Parent1.Children.Remove(Child1), "Removed Child1 from Parent1");
        session.Commit();
        Assert.IsNull(Child1.Parent, "Child1.Parent after Remove #1");
        Assert.AreSame(Parent1, Child2.Parent, "Child2.Parent after Remove #1");
        Assert.AreSame(Child2, Parent1.Children[0], "Parent1 1st child after Remove #1");
        Assert.AreEqual(1, Parent1.Children.Count,
          "Parent1.Children.Count after Remove #1");
        Assert.AreEqual(1, Parent1.References.Count,
          "Parent1.References.Count after Remove #1");
        session.BeginUpdate();
        Assert.IsTrue(Parent1.Children.Remove(Child2), "Removed Child2 from Parent1");
        session.Commit();
        Assert.IsNull(Child2.Parent, "Child2.Parent after Remove #2");
        Assert.AreEqual(0, Parent1.Children.Count,
          "Parent1.Children.Count after Remove #2");
        Assert.AreEqual(0, Parent1.References.Count,
          "Parent1.References.Count after Remove #2");
        session.BeginUpdate();
        Child1.Unpersist(session);
        Child2.Unpersist(session);
        Parent1.Unpersist(session);
        session.Commit();
        Assert.IsFalse(Child1.IsPersistent, "Child1.IsPersistent after Unpersist");
        Assert.IsFalse(Child2.IsPersistent, "Child2.IsPersistent after Unpersist");
        Assert.IsFalse(Parent1.IsPersistent, "Parent1.IsPersistent after Unpersist");
      }
    }

    [Test]
    public void T50_ChangeParent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent1 = Parent.Read(Parent1Name, session);
        Parent2 = Parent.Read(Parent2Name, session);
        Child1 = Child.Read(Child1Name, session);
        Child1.Parent = Parent2;
        session.Commit();
        Assert.AreSame(Parent2, Child1.Parent, "Child1.Parent after change parent");
        Assert.AreEqual(0, Parent1.Children.Count,
          "Parent1.Children.Count after change parent");
        Assert.AreEqual(0, Parent1.References.Count,
          "Parent1.References.Count after change parent");
        Assert.AreEqual(1, Parent2.Children.Count,
          "Parent2.Children.Count after change parent");
        Assert.AreEqual(1, Parent2.References.Count,
          "Parent2.References.Count after change parent");
        Assert.AreSame(Child1, Parent2.Children[0],
          "Parent2 1st child after change parent");
      }
    }

    [Test]
    public void T60_ChangeParentFromNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent2 = Parent.Read(Parent2Name, session);
        Child2 = Child.Read(Child2Name, session);
        Child2.Parent = Parent2;
        session.Commit();
        Assert.AreSame(Parent2, Child2.Parent, "Child2.Parent");
        Assert.AreEqual(1, Parent2.Children.Count,
          "Parent2.Children.Count");
        Assert.AreEqual(1, Parent2.References.Count,
          "Parent2.References.Count");
        Assert.AreSame(Child2, Parent2.Children[0],
          "Parent2 1st child after change parent");
      }
    }
    [Test]
    public void T70_ChangeParentToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent1 = Parent.Read(Parent1Name, session);
        Child1 = Child.Read(Child1Name, session);
        Child1.Parent = null;
        session.Commit();
        Assert.IsNull(Child1.Parent, "Child1.Parent");
        Assert.AreEqual(0, Parent1.Children.Count,
          "Parent1.Children.Count");
        Assert.AreEqual(0, Parent1.References.Count,
          "Parent2.References.Count");
      }
    }

    [Test]
    public void T80_DeleteReferencingChild() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Parent1 = Parent.Read(Parent1Name, session);
        Child1 = Child.Read(Child1Name, session);
        Child1.Unpersist(session);
        session.Commit();
        Assert.AreEqual(0, Parent1.Children.Count,
          "Parent1.Children.Count");
        Assert.AreEqual(0, Parent1.References.Count,
          "Parent2.References.Count");
      }
    }
  }
}