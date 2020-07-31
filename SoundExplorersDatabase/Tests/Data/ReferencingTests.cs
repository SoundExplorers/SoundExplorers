using System;
using System.Collections.Generic;
using System.Data;
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
      Father1 = new Father();
      Father2 = new Father {Name = Father2Name};
      Son1 = new Son();
      Son2 = new Son();
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
        session.Persist(Father1);
        Father1.Name = Father1Name;
        session.Persist(Father2);
        session.Persist(Son1);
        Son1.Name = Son1Name;
        session.Persist(Son2);
        Son2.Name = Son2Name;
        Assert.IsTrue(Mother1.Daughters.Add(Daughter1),
          "Added Daughter1 to Mother1");
        Assert.IsTrue(Mother1.Sons.Add(Son1),
          "Added Son1 to Mother1");
        Assert.IsTrue(Mother1.Sons.Add(Son2),
          "Added Son2 to Mother1");
        Assert.IsTrue(Father1.Sons.Add(Son1),
          "Added Son1 to Father1");
        Assert.IsTrue(Father1.Daughters.Add(Daughter1),
          "Added Daughter1 to Father1");
        Assert.IsTrue(Father1.Daughters.Add(Daughter2),
          "Added Daughter2 to Father1");
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string Daughter1Name = "Alison";
    private const string Daughter2Name = "Bertha the Cool";
    private const string Father1Name = "Arthur";
    private const string Father2Name = "Brendan";
    private const string Mother1Name = "Adrienne";
    private const string Mother2Name = "Barbara";
    private const string Son1Name = "Adrian";
    private const string Son2Name = "Barry";

    private Daughter Daughter1 { get; set; }
    private Daughter Daughter2 { get; set; }
    private string DatabaseFolderPath { get; set; }
    private Father Father1 { get; set; }
    private Father Father2 { get; set; }
    private Mother Mother1 { get; set; }
    private Mother Mother2 { get; set; }
    private Son Son1 { get; set; }
    private Son Son2 { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Mother1 = Mother.Read(Mother1Name, session);
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Father1 = Father.Read(Father1Name, session);
        Father2 = Father.Read(Father2Name, session);
        Son1 = Son.Read(Son1Name, session);
        Son2 = Son.Read(Son2Name, session);
        session.Commit();
        Assert.IsTrue(Daughter1.IsPersistent,
          "Daughter1.IsPersistent initially");
        Assert.AreEqual(Daughter1Name, Daughter1.Name, "Daughter1.Name");
        Assert.IsTrue(Daughter2.IsPersistent,
          "Daughter2.IsPersistent initially");
        Assert.AreEqual(Daughter2Name, Daughter2.Name, "Daughter2.Name");
        Assert.IsTrue(Mother1.IsPersistent, "Mother1.IsPersistent initially");
        Assert.AreEqual(Mother1Name, Mother1.Name, "Mother1.Name initially");
        Assert.IsTrue(Mother2.IsPersistent, "Mother2.IsPersistent initially");
        Assert.AreEqual(Mother2Name, Mother2.Name, "Mother2.Name initially");
        Assert.AreEqual(1, Mother1.Daughters.Count,
          "Mother1.Daughters.Count initially");
        Assert.AreEqual(2, Mother1.Sons.Count,
          "Mother1.Sons.Count initially");
        Assert.AreEqual(3, Mother1.References.Count,
          "Mother1.References.Count initially");
        Assert.AreSame(Daughter1, Mother1.Daughters[0],
          "Mother1 1st Daughter initially");
        Assert.AreSame(Daughter1, Mother1.Daughters[Daughter1Name],
          "Mother1 1st Daughter by name initially");
        Assert.AreSame(Mother1, Daughter1.Mother, "Daughter1.Mother initially");
        Assert.AreSame(Son1, Mother1.Sons[0],
          "Mother1 1st Son initially");
        Assert.AreSame(Son1, Mother1.Sons[Son1Name],
          "Mother1 1st Son by name initially");
        Assert.AreSame(Mother1, Son1.Mother, "Son1.Mother initially");
        Assert.AreSame(Son2, Mother1.Sons[1],
          "Mother1 2nd Son initially");
        Assert.AreSame(Son2, Mother1.Sons[Son2Name],
          "Mother1 2nd Son by name initially");
        Assert.AreSame(Mother1, Son2.Mother, "Son2.Mother initially");
        Assert.AreEqual(Mother2Name, Mother2.Name, "Mother2.Name initially");
        Assert.AreEqual(0, Mother2.Daughters.Count,
          "Mother2.Daughters.Count initially");
        Assert.AreEqual(0, Mother2.References.Count,
          "Mother2.References.Count initially");
        Assert.IsNull(Daughter2.Mother, "Daughter2.Mother initially");
        Assert.AreEqual(0, Mother2.Daughters.Count,
          "Mother2.Daughters.Count initially");
        Assert.AreEqual(0, Mother2.Sons.Count,
          "Mother2.Sons.Count initially");
        Assert.AreEqual(0, Mother2.References.Count,
          "Mother2.References.Count initially");
        Assert.IsNull(Daughter2.Mother, "Daughter2.Mother initially");
        
        Assert.IsTrue(Son1.IsPersistent,
          "Son1.IsPersistent initially");
        Assert.AreEqual(Son1Name, Son1.Name, "Son1.Name");
        Assert.IsTrue(Son2.IsPersistent,
          "Son2.IsPersistent initially");
        Assert.AreEqual(Son2Name, Son2.Name, "Son2.Name");
        Assert.IsTrue(Father1.IsPersistent, "Father1.IsPersistent initially");
        Assert.AreEqual(Father1Name, Father1.Name, "Father1.Name initially");
        Assert.IsTrue(Father2.IsPersistent, "Father2.IsPersistent initially");
        Assert.AreEqual(Father2Name, Father2.Name, "Father2.Name initially");
        Assert.AreEqual(1, Father1.Sons.Count,
          "Father1.Sons.Count initially");
        Assert.AreEqual(2, Father1.Daughters.Count,
          "Father1.Daughters.Count initially");
        Assert.AreEqual(3, Father1.References.Count,
          "Father1.References.Count initially");
        Assert.AreSame(Son1, Father1.Sons[0],
          "Father1 1st Son initially");
        Assert.AreSame(Son1, Father1.Sons[Son1Name],
          "Father1 1st Son by name initially");
        Assert.AreSame(Father1, Son1.Father, "Son1.Father initially");
        Assert.AreSame(Daughter1, Father1.Daughters[0],
          "Father1 1st Daughter initially");
        Assert.AreSame(Daughter1, Father1.Daughters[Daughter1Name],
          "Father1 1st Daughter by name initially");
        Assert.AreSame(Father1, Daughter1.Father, "Daughter1.Father initially");
        Assert.AreSame(Daughter2, Father1.Daughters[1],
          "Father1 2nd Daughter initially");
        Assert.AreSame(Daughter2, Father1.Daughters[Daughter2Name],
          "Father1 2nd Daughter by name initially");
        Assert.AreSame(Father1, Daughter2.Father, "Daughter2.Father initially");
        Assert.AreEqual(Father2Name, Father2.Name, "Father2.Name initially");
        Assert.AreEqual(0, Father2.Sons.Count,
          "Father2.Sons.Count initially");
        Assert.AreEqual(0, Father2.References.Count,
          "Father2.References.Count initially");
        Assert.IsNull(Son2.Father, "Son2.Father initially");
        Assert.AreEqual(0, Father2.Sons.Count,
          "Father2.Sons.Count initially");
        Assert.AreEqual(0, Father2.Daughters.Count,
          "Father2.Daughters.Count initially");
        Assert.AreEqual(0, Father2.References.Count,
          "Father2.References.Count initially");
        Assert.IsNull(Son2.Father, "Son2.Father initially");
      }
    }

    [Test]
    public void T020_SortedChildListUnsupportedMethods() {
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
    public void T030_CannotDeleteParentWithChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<ReferentialIntegrityException>(() =>
          Mother1.Unpersist(session));
        session.Commit();
      }
    }

    [Test]
    public void T035_CannotAddChildWithParentToAnotherParentOfSameType() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Father2 = Father.Read(Father2Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Assert.Throws<ConstraintException>(() =>
          Father2.Daughters.Add(Daughter2));
        session.Commit();
      }
    }

    [Test]
    public void T037_CannotRemoveChildThatDoesNotBelongToParent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Father1 = Father.Read(Father1Name, session);
        Son1 = Son.Read(Son1Name, session);
        Father1.Sons.Remove(Son1);
        Assert.Throws<KeyNotFoundException>(() =>
          Father1.Sons.Remove(Son1));
        session.Commit();
      }
    }

    [Test]
    public void T040_AddRemoveChildren() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Father1 = Father.Read(Father1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Son1 = Son.Read(Son1Name, session);
        Son2 = Son.Read(Son2Name, session);
        Assert.IsTrue(Mother1.Daughters.Add(Daughter2),
          "Added Daughter2 to Mother1");
        Assert.IsTrue(Father1.Sons.Add(Son2),
          "Added Son2 to Father1");
        session.Commit();
        
        Assert.AreSame(Mother1, Daughter2.Mother,
          "Daughter2.Mother after Add #2");
        Assert.AreSame(Daughter2, Mother1.Daughters[1],
          "2nd child after Add #2");
        Assert.AreEqual(2, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Add #2");
        Assert.AreEqual(4, Mother1.References.Count,
          "Mother1.References.Count after Add #2");
        session.BeginUpdate();
        Assert.IsTrue(Mother1.Daughters.Remove(Daughter1),
          "Removed Daughter1 from Mother1");
        session.Commit();
        Assert.IsNull(Daughter1.Mother, "Daughter1.Mother after Remove #1");
        Assert.AreSame(Mother1, Daughter2.Mother,
          "Daughter2.Mother after Remove #1");
        Assert.AreSame(Daughter2, Mother1.Daughters[0],
          "Mother1 1st Daughter after Remove #1");
        Assert.AreEqual(1, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Remove #1");
        Assert.AreEqual(3, Mother1.References.Count,
          "Mother1.References.Count after Remove #1");
        session.BeginUpdate();
        Assert.IsTrue(Mother1.Daughters.Remove(Daughter2),
          "Removed Daughter2 from Mother1");
        session.Commit();
        Assert.IsNull(Daughter2.Mother, "Daughter2.Mother after Remove #2");
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Remove #2");
        Assert.AreEqual(2, Mother1.References.Count,
          "Mother1.References.Count after Remove #2");

        Assert.AreSame(Father1, Son2.Father,
          "Son2.Father after Add #2");
        Assert.AreSame(Son2, Father1.Sons[1],
          "2nd child after Add #2");
        Assert.AreEqual(2, Father1.Sons.Count,
          "Father1.Sons.Count after Add #2");
        Assert.AreEqual(4, Father1.References.Count,
          "Father1.References.Count after Add #2");
        session.BeginUpdate();
        Assert.IsTrue(Father1.Sons.Remove(Son1),
          "Removed Son1 from Father1");
        session.Commit();
        Assert.IsNull(Son1.Father, "Son1.Father after Remove #1");
        Assert.AreSame(Father1, Son2.Father,
          "Son2.Father after Remove #1");
        Assert.AreSame(Son2, Father1.Sons[0],
          "Father1 1st Son after Remove #1");
        Assert.AreEqual(1, Father1.Sons.Count,
          "Father1.Sons.Count after Remove #1");
        Assert.AreEqual(3, Father1.References.Count,
          "Father1.References.Count after Remove #1");
        session.BeginUpdate();
        Assert.IsTrue(Father1.Sons.Remove(Son2),
          "Removed Son2 from Father1");
        session.Commit();
        Assert.IsNull(Son2.Father, "Son2.Father after Remove #2");
        Assert.AreEqual(0, Father1.Sons.Count,
          "Father1.Sons.Count after Remove #2");
        Assert.AreEqual(2, Father1.References.Count,
          "Father1.References.Count after Remove #2");

        session.BeginUpdate();
        Daughter1.Unpersist(session);
        Daughter2.Unpersist(session);
        Mother1.Sons.Remove(Son1);
        Mother1.Sons.Remove(Son2);
        Mother1.Unpersist(session);
        Father1.Unpersist(session);
        session.Commit();
        Assert.IsFalse(Daughter1.IsPersistent,
          "Daughter1.IsPersistent after Unpersist");
        Assert.IsFalse(Daughter2.IsPersistent,
          "Daughter2.IsPersistent after Unpersist");
        Assert.IsFalse(Mother1.IsPersistent,
          "Mother1.IsPersistent after Unpersist");
        Assert.IsFalse(Father1.IsPersistent,
          "Father1.IsPersistent after Unpersist");
      }
    }

    [Test]
    public void T050_ChangeParent() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Father1 = Father.Read(Father1Name, session);
        Father2 = Father.Read(Father2Name, session);
        Son1 = Son.Read(Son1Name, session);
        Daughter1.Mother = Mother2;
        Daughter1.Father = Father2;
        Son1.Mother = Mother2;
        Son1.Father = Father2;
        session.Commit();
        Assert.AreSame(Father2, Daughter1.Father,
          "Daughter1.Father after Daughter1 changes Father");
        Assert.AreSame(Mother2, Daughter1.Mother,
          "Daughter1.Mother after Daughter1 changes Mother");
        Assert.AreSame(Father2, Son1.Father,
          "Son1.Father after Son1 changes Father");
        Assert.AreSame(Mother2, Son1.Mother,
          "Son1.Mother after Son1 changes Mother");

        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count after Daughter1 changes Mother");
        Assert.AreEqual(1, Mother1.Sons.Count,
          "Mother1.Sons.Count after Son1 changes Mother");
        Assert.AreEqual(1, Mother1.References.Count,
          "Mother1.References.Count after Daughter1 and Son1 change Mother");
        Assert.AreEqual(1, Mother2.Daughters.Count,
          "Mother2.Daughters.Count after Daughter1 changes Mother");
        Assert.AreEqual(1, Mother2.Sons.Count,
          "Mother2.Sons.Count after Son1 changes Mother");
        Assert.AreEqual(2, Mother2.References.Count,
          "Mother2.References.Count after Daughter1 and Son1 change Mother");
        Assert.AreSame(Daughter1, Mother2.Daughters[0],
          "Mother2 1st Daughter after Daughter1 changes Mother");
        Assert.AreSame(Son1, Mother2.Sons[0],
          "Mother2 1st Son after Son1 changes Mother");

        Assert.AreEqual(0, Father1.Sons.Count,
          "Father1.Sons.Count after Son1 changes Father");
        Assert.AreEqual(1, Father1.Daughters.Count,
          "Father1.Daughters.Count after Daughter1 changes Father");
        Assert.AreEqual(1, Father1.References.Count,
          "Father1.References.Count after Son1 and Daughter1 change Father");
        Assert.AreEqual(1, Father2.Sons.Count,
          "Father2.Sons.Count after Son1 changes Father");
        Assert.AreEqual(1, Father2.Daughters.Count,
          "Father2.Daughters.Count after Daughter1 changes Father");
        Assert.AreEqual(2, Father2.References.Count,
          "Father2.References.Count after Son1 and Daughter1 change Father");
        Assert.AreSame(Son1, Father2.Sons[0],
          "Father2 1st Son after Son1 changes Father");
        Assert.AreSame(Daughter1, Father2.Daughters[0],
          "Father2 1st Daughter after Daughter1 changes Father");
      }
    }

    [Test]
    public void T060_ChangeParentFromNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother2 = Mother.Read(Mother2Name, session);
        Daughter2 = Daughter.Read(Daughter2Name, session);
        Daughter2.Mother = Mother2;
        
        Father2 = Father.Read(Father2Name, session);
        Son2 = Son.Read(Son2Name, session);
        Son2.Father = Father2;
        session.Commit();
        Assert.AreSame(Mother2, Daughter2.Mother, "Daughter2.Mother");
        Assert.AreEqual(1, Mother2.Daughters.Count,
          "Mother2.Daughters.Count");
        Assert.AreEqual(1, Mother2.References.Count,
          "Mother2.References.Count");
        Assert.AreSame(Daughter2, Mother2.Daughters[0],
          "Mother2 1st child after change Mother");
        
        Assert.AreSame(Father2, Son2.Father, "Son2.Father");
        Assert.AreEqual(1, Father2.Sons.Count,
          "Father2.Sons.Count");
        Assert.AreEqual(1, Father2.References.Count,
          "Father2.References.Count");
        Assert.AreSame(Son2, Father2.Sons[0],
          "Father2 1st child after change Father");
      }
    }

    [Test]
    public void T070_ChangeParentToNull() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Daughter1.Mother = null;
        
        Father1 = Father.Read(Father1Name, session);
        Son1 = Son.Read(Son1Name, session);
        Son1.Father = null;
        session.Commit();
        Assert.IsNull(Daughter1.Mother, "Daughter1.Mother");
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count");
        Assert.AreEqual(2, Mother1.References.Count,
          "Mother2.References.Count");
        
        Assert.IsNull(Son1.Father, "Son1.Father");
        Assert.AreEqual(0, Father1.Sons.Count,
          "Father1.Sons.Count");
        Assert.AreEqual(2, Father1.References.Count,
          "Father2.References.Count");
      }
    }

    [Test]
    public void T080_DeleteReferencingChild() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Mother1 = Mother.Read(Mother1Name, session);
        Daughter1 = Daughter.Read(Daughter1Name, session);
        Father1 = Father.Read(Father1Name, session);
        Son1 = Son.Read(Son1Name, session);
        Daughter1.Unpersist(session);
        
        Father1 = Father.Read(Father1Name, session);
        Son1 = Son.Read(Son1Name, session);
        Son1.Unpersist(session);
        session.Commit();
        Assert.AreEqual(0, Mother1.Daughters.Count,
          "Mother1.Daughters.Count");
        Assert.AreEqual(1, Mother1.Sons.Count,
          "Mother1.Sons.Count");
        Assert.AreEqual(1, Mother1.References.Count,
          "Mother1.References.Count");
        
        Assert.AreEqual(0, Father1.Sons.Count,
          "Father1.Sons.Count");
        Assert.AreEqual(1, Father1.Daughters.Count,
          "Father1.Sons.Count");
        Assert.AreEqual(1, Father1.References.Count,
          "Father2.References.Count");
      }
    }
  }
}