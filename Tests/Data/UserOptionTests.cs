using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data {
  [TestFixture]
  public class UserOptionTests : TestFixtureBase {
    [SetUp]
    public override void Setup() {
      base.Setup();
      UserOption1 = new UserOption {
        QueryHelper = QueryHelper,
        UserId = UserOption1UserId,
        OptionName = UserOption1OptionName,
        OptionValue = UserOption1OptionValue
      };
      Session.BeginUpdate();
      Session.Persist(UserOption1);
      Session.Commit();
      Session.BeginRead();
      UserOption1 = QueryHelper.Read<UserOption>(UserOptionSimpleKey, Session);
      Session.Commit();
    }

    private const string UserOption1OptionName = "ChalkOrCheese";
    private const string UserOption1OptionValue = "Cheese, please.";
    private const string UserOptionSimpleKey = "Alice|ChalkOrCheese";
    private const string UserOption1UserId = "Alice";
    private UserOption UserOption1 { get; set; } = null!;

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
      Assert.Throws<PropertyConstraintException>(() => UserOption1.UserId = string.Empty,
        "UserOption1.UserId = Empty");
      Assert.Throws<PropertyConstraintException>(
        () => UserOption1.OptionName = string.Empty,
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
      Assert.Throws<PropertyConstraintException>(() =>
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
      Assert.Throws<PropertyConstraintException>(() => Session.Persist(duplicate));
      Session.Commit();
    }
  }
}