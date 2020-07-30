using System;
using NUnit.Framework;
using VelocityDb.Exceptions;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class ReferencingTests {
    [SetUp]
    public void Setup() {
      Mother1 = new Mother();
      Mother2 = new Mother {Name = Mother2Name};
      Daughter1 = new Daughter();
      Daughter2 = new Daughter();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Mother1);
        Mother1.Name = Mother1Name;
        session.Persist(Mother2);
        session.Persist(Daughter1);
        Daughter1.Name = Daughter1Name;
        session.Persist(Daughter2);
        Daughter2.Name = Daughter2Name;
        Assert.IsTrue(Mother1.Daughters.Add(Daughter1),
          "Added Daughter1 to Mother1");
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Daughter1Name = "Alison";
    private const string Daughter2Name = "Bertha the Cool";
    private const string Mother1Name = "Adrienne";
    private const string Mother2Name = "Barbara";

    private Daughter Daughter1 { get; set; }
    private Daughter Daughter2 { get; set; }
    private string DatabaseFolderPath { get; set; }
    private Mother Mother1 { get; set; }
    private Mother Mother2 { get; set; }

    [Test]
    public void T10_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Mother1 = Mother.Read(Mother1Name, session);
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        session.Commit();
        Assert.IsTrue(Daughter1.IsPersistent,
          "Daughter1.IsPersistent initially");
        Assert.AreEqual(Daughter1Name, Daughter1.Name, "Daughter1.Name");
        Assert.IsTrue(Mother1.IsPersistent, "Mother1.IsPersistent initially");
        Assert.AreEqual(Mother1Name, Mother1.Name, "Mother1.Name initially");
        Assert.AreEqual(1, Mother1.Daughters.Count,
          "Mother1.Daughters.Count initially");
        Assert.AreEqual(1, Mother1.References.Count,
          "Mother1.References.Count after Add #1");
        Assert.AreEqual(1, Mother1.Daughters.Count,
          "Mother1.Daughters.Count initially");
        Assert.AreEqual(1, Mother1.References.Count,
          "Mother1.References.Count initially");
        Assert.AreSame(Daughter1, Mother1.Daughters[0],
          "Mother1 1st child initially");
        Assert.AreSame(Daughter1, Mother1.Daughters[Daughter1Name],
          "1st child by name initially");
        Assert.AreSame(Mother1, Daughter1.Mother, "Daughter1.Mother initially");
        Assert.AreEqual(Mother2Name, Mother2.Name, "Mother2.Name initially");
        Assert.AreEqual(0, Mother2.Daughters.Count,
          "Mother2.Daughters.Count initially");
        Assert.AreEqual(0, Mother2.References.Count,
          "Mother2.References.Count initially");
        Assert.IsNull(Daughter2.Mother, "Daughter2.Mother initially");
      }
    }

    [Test]
    public void T20_SortedChildListUnsupportedMethods() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Assert.Throws<NotSupportedException>(
          () => Mother1.Daughters.Add(Daughter2Name, Daughter2),
          "Unsupported Mother.Daughters.Add");
        Assert.Throws<NotSupportedException>(
          () => Mother1.Daughters.Remove(Daughter1Name),
          "Unsupported Mother.Daughters.Remove");
        Assert.Throws<NotSupportedException>(
          () => Mother1.Daughters.RemoveAt(0),
          "Unsupported Mother.Daughters.RemoveAt");
        session.Commit();
      }
    }

    [Test]
    public void T30_CannotDeleteParentWithChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() =>
          Mother1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void T40_AddRemoveChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Assert.IsTrue(Mother1.Daughters.Add(Daughter2),
          "Added Daughter2 to Mother1");
        session.Commit();
        Assert.AreSame(Mother1, Daughter2.Mother,
          "Daughter2.Mother after Add #2");
        Assert.AreSame(Daughter2, Mother1.Daughters[1],
          "2nd child after Add #2");
        Assert.AreEqual(2, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Add #2");
        Assert.AreEqual(2, Mother1.References.Count,
          "Mother1.References.Count after Add #2");
        session.BeginUpdate();
        Assert.IsTrue(Mother1.Daughters.Remove(Daughter1),
          "Removed Daughter1 from Mother1");
        session.Commit();
        Assert.IsNull(Daughter1.Mother, "Daughter1.Mother after Remove #1");
        Assert.AreSame(Mother1, Daughter2.Mother,
          "Daughter2.Mother after Remove #1");
        Assert.AreSame(Daughter2, Mother1.Daughters[0],
          "Mother1 1st child after Remove #1");
        Assert.AreEqual(1, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Remove #1");
        Assert.AreEqual(1, Mother1.References.Count,
          "Mother1.References.Count after Remove #1");
        session.BeginUpdate();
        Assert.IsTrue(Mother1.Daughters.Remove(Daughter2),
          "Removed Daughter2 from Mother1");
        session.Commit();
        Assert.IsNull(Daughter2.Mother, "Daughter2.Mother after Remove #2");
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Remove #2");
        Assert.AreEqual(0, Mother1.References.Count,
          "Mother1.References.Count after Remove #2");
        session.BeginUpdate();
        Daughter1.Unpersist(session);
        Daughter2.Unpersist(session);
        Mother1.Unpersist(session);
        session.Commit();
        Assert.IsFalse(Daughter1.IsPersistent,
          "Daughter1.IsPersistent after Unpersist");
        Assert.IsFalse(Daughter2.IsPersistent,
          "Daughter2.IsPersistent after Unpersist");
        Assert.IsFalse(Mother1.IsPersistent,
          "Mother1.IsPersistent after Unpersist");
      }
    }

    [Test]
    public void T50_ChangeParent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter1.Mother = Mother2;
        session.Commit();
        Assert.AreSame(Mother2, Daughter1.Mother,
          "Daughter1.Mother after change Mother");
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after change Mother");
        Assert.AreEqual(0, Mother1.References.Count,
          "Mother1.References.Count after change Mother");
        Assert.AreEqual(1, Mother2.Daughters.Count,
          "Mother2.Daughters.Count after change Mother");
        Assert.AreEqual(1, Mother2.References.Count,
          "Mother2.References.Count after change Mother");
        Assert.AreSame(Daughter1, Mother2.Daughters[0],
          "Mother2 1st child after change Mother");
      }
    }

    [Test]
    public void T60_ChangeParentFromNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Daughter2.Mother = Mother2;
        session.Commit();
        Assert.AreSame(Mother2, Daughter2.Mother, "Daughter2.Mother");
        Assert.AreEqual(1, Mother2.Daughters.Count,
          "Mother2.Daughters.Count");
        Assert.AreEqual(1, Mother2.References.Count,
          "Mother2.References.Count");
        Assert.AreSame(Daughter2, Mother2.Daughters[0],
          "Mother2 1st child after change Mother");
      }
    }

    [Test]
    public void T70_ChangeParentToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter1.Mother = null;
        session.Commit();
        Assert.IsNull(Daughter1.Mother, "Daughter1.Mother");
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count");
        Assert.AreEqual(0, Mother1.References.Count,
          "Mother2.References.Count");
      }
    }

    [Test]
    public void T80_DeleteReferencingChild() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter1.Unpersist(session);
        session.Commit();
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count");
        Assert.AreEqual(0, Mother1.References.Count,
          "Mother2.References.Count");
      }
    }
  }
}