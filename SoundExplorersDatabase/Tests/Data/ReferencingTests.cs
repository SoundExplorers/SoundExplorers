using System.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;
using VelocityDb.Exceptions;
using VelocityDb.Session;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class ReferencingTests {
    private const string Child1Name = "Alison";
    private const string Child2Name = "Bertha the Cool";
    private const string Parent1Name = "Adrienne";
    private const string Parent2Name = "Bertram";
    private Child Child1 { get; set; }
    private Child Child2 { get; set; }
    private Parent Parent1 { get; set; }
    private Parent Parent2 { get; set; }

    [SetUp]
    public void Setup() {
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Child1 = new Child();
      Child2 = new Child();
      Parent1 = new Parent();
      Parent2 = new Parent();
      using (var session = new SessionNoServer(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Child1);
        Child1.Name = Child1Name;
        session.Persist(Parent1);
        Parent1.Name = Parent1Name;
        session.Persist(Parent2);
        Parent2.Name = Parent2Name;
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }
    
    private string DatabaseFolderPath { get; set; }

    [Test]
    public void Test1() {
      using (var session = new SessionNoServer(DatabaseFolderPath)) {
        session.BeginUpdate();
        Child1 = Child.Read(Child1Name, session);
        Parent1 = Parent.Read(Parent1Name, session);
        Parent1.Children.Add(Child1);
        Assert.AreSame(Child1, Parent1.Children.First(), "1st child after Add #1");
        session.Commit();
        Assert.AreSame(Parent1, Child1.Parent, "child1.Parent after Add #1");
        Assert.IsTrue(Child1.IsPersistent, "child1.IsPersistent after Persist");
        Assert.AreEqual(Child1Name, Child1.Name, "child1.Name");
        Assert.IsTrue(Parent1.IsPersistent, "parent1.IsPersistent after Persist");
        Assert.AreEqual(Parent1Name, Parent1.Name, "parent1.Name");
        Assert.AreEqual(1, Parent1.Children.Count, "parent1.Children.Count after Add #1");
        Assert.AreEqual(1, Parent1.References.Count,
          "parent1.References.Count after Add #1");
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() => Parent1.Unpersist(session));
        session.Persist(Child2);
        Child2.Name = Child2Name;
        session.Commit();
        session.BeginUpdate();
        Child2 = Child.Read(Child2Name, session);
        Parent1.Children.Add(Child2);
        Assert.AreSame(Child2, Parent1.Children.ToArray()[1], "2nd child after Add #2");
        session.Commit();
        Assert.AreSame(Parent1, Child2.Parent, "child2.Parent after Add #2");
        Assert.AreEqual(2, Parent1.Children.Count, "parent1.Children.Count after Add #2");
        Assert.AreEqual(2, Parent1.References.Count,
          "parent1.References.Count after Add #2");
        session.BeginUpdate();
        Parent1.Children.Remove(Child1);
        Assert.AreSame(Child2, Parent1.Children.First(), "1st child after Remove #1");
        session.Commit();
        Assert.IsNull(Child1.Parent, "child1.Parent after Remove #1");
        Assert.AreEqual(1, Parent1.Children.Count,
          "parent1.Children.Count after Remove #1");
        Assert.AreEqual(1, Parent1.References.Count,
          "parent1.References.Count after Remove #1");
        session.BeginUpdate();
        Parent2 = Parent.Read(Parent2Name, session);
        Child2.Parent = Parent2;
        Assert.AreSame(Child2, Parent2.Children.First(), "Parent 2 1st child after move");
        session.Commit();
        Assert.AreSame(Parent2, Child2.Parent, "child2.Parent after changing parent");
        // Assert.AreEqual(0, parent1.Children.Count,
        //   "parent1.Children.Count after changing parent");
        // Assert.AreEqual(0, parent1.References.Count,
        //   "parent1.References.Count after changing parent");
        Assert.AreEqual(1, Parent2.Children.Count,
          "parent2.Children.Count after changing parent");
        Assert.AreEqual(1, Parent2.References.Count,
          "parent2.References.Count after changing parent");
        session.BeginUpdate();
        Child1.Unpersist(session);
        Child2.Unpersist(session);
        //parent1.Unpersist(session);
        //parent2.Unpersist(session);
        session.Commit();
        Assert.IsFalse(Child1.IsPersistent, "child1.IsPersistent after Unpersist");
        Assert.IsFalse(Child2.IsPersistent, "child2.IsPersistent after Unpersist");
        //Assert.IsFalse(parent1.IsPersistent, "parent1.IsPersistent after Unpersist");
        //Assert.IsFalse(parent2.IsPersistent, "parent2.IsPersistent after Unpersist");
      }
    }
  }
}