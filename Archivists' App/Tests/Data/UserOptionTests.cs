using System.Data;
using System.Data.Linq;
using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class UserOptionTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      DatabaseFolderPath = TestSession.CreateDatabaseFolder();
      UserOption1 = new UserOption {
        QueryHelper = QueryHelper,
        UserId = UserOption1UserId,
        OptionName = UserOption1OptionName,
        OptionValue = UserOption1OptionValue
      };
      using (var session = new TestSession(DatabaseFolderPath)) {
        session.BeginUpdate();
        session.Persist(UserOption1);
        session.Commit();
      }
      Session = new TestSession(DatabaseFolderPath);
      Session.BeginRead();
      UserOption1 = QueryHelper.Read<UserOption>(UserOptionSimpleKey, Session);
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      TestSession.DeleteFolderIfExists(DatabaseFolderPath);
    }

    private const string UserOption1OptionName = "ChalkOrCheese";
    private const string UserOption1OptionValue = "Cheese, please.";
    private const string UserOptionSimpleKey = "Alice|ChalkOrCheese";
    private const string UserOption1UserId = "Alice";
    private string DatabaseFolderPath { get; set; }
    private QueryHelper QueryHelper { get; set; }
    private TestSession Session { get; set; }
    private UserOption UserOption1 { get; set; }

    [Test]
    public void A010_Initial() {
      Assert.AreEqual(UserOption1UserId, UserOption1.UserId, "UserOption1.UserId");
      Assert.AreEqual(UserOption1OptionName, UserOption1.OptionName,
        "UserOption1.OptionName");
      Assert.AreEqual(UserOption1OptionValue, UserOption1.OptionValue,
        "UserOption1.OptionValue");
    }

    [Test]
    public void DisallowBlankKeyPropertyValues() {
      Assert.Throws<NoNullAllowedException>(() => UserOption1.UserId = null,
        "UserOption1.UserId = null");
      Assert.Throws<NoNullAllowedException>(() => UserOption1.UserId = string.Empty,
        "UserOption1.UserId = Empty");
      Assert.Throws<NoNullAllowedException>(() => UserOption1.OptionName = null,
        "UserOption1.OptionName = null");
      Assert.Throws<NoNullAllowedException>(() => UserOption1.OptionName = string.Empty,
        "UserOption1.OptionName = Empty");
    }

    [Test]
    public void DisallowChangeToDuplicate() {
      var userOption2 = new UserOption {
        QueryHelper = QueryHelper,
        UserId = UserOption1UserId,
        OptionName = "Different"
      };
      Session.BeginUpdate();
      Session.Persist(userOption2);
      Assert.Throws<DuplicateKeyException>(() =>
        userOption2.OptionName = UserOption1OptionName);
      Session.Commit();
    }

    [Test]
    public void DisallowPersistDuplicate() {
      // Tests that comparison is case-insensitive.
      var duplicate = new UserOption {
        QueryHelper = QueryHelper,
        UserId = "alice",
        // ReSharper disable once StringLiteralTypo
        OptionName = "chalkorcheese"
      };
      Session.BeginUpdate();
      Assert.Throws<DuplicateKeyException>(() => Session.Persist(duplicate));
      Session.Commit();
    }
  }
}