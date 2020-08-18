using System.Data.Linq;
using NUnit.Framework;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase.Tests.Data {
  [TestFixture]
  public class RoleTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      Drums = new Role {
        QueryHelper = QueryHelper,
        Name = DrumsName
      };
      ElectricGuitar = new Role {
        QueryHelper = QueryHelper,
        Name = ElectricGuitarName
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(Drums);
        session.Persist(ElectricGuitar);
        session.Commit();
      }
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string DrumsName = "Drums";
    private const string ElectricGuitarName = "Electric guitar";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private Role Drums { get; set; }
    private Role ElectricGuitar { get; set; }

    [Test]
    public void T010_Initial() {
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginRead();
        Drums = QueryHelper.Read<Role>(DrumsName, session);
        ElectricGuitar = QueryHelper.Read<Role>(ElectricGuitarName, session);
        session.Commit();
      }
      Assert.AreEqual(DrumsName, Drums.Name, "Drums.Name initially");
      Assert.AreEqual(ElectricGuitarName, ElectricGuitar.Name,
        "ElectricGuitar.Name initially");
    }

    [Test]
    public void T020_DisallowDuplicate() {
      var duplicate = new Role {
        QueryHelper = QueryHelper,
        Name = "drums" // Tests that comparison is case-insensitive.
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        Assert.Throws<DuplicateKeyException>(() => session.Persist(duplicate));
        session.Commit();
      }
    }
  }
}