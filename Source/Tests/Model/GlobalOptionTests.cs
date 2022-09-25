using System;
using NUnit.Framework;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model; 

[TestFixture]
public class GlobalOptionTests : TestFixtureBase {

  [Test]
  public void TheTest() {
    var newDateTime = DateTime.Parse("2099-12-31 23:59:59");
    var option = CreateTestGlobalOption("DateTime", newDateTime);
    Assert.AreEqual(newDateTime, option.DateTimeValue, "DateTimeValue");
    Assert.AreEqual("ˉGlobal", option.UserOption.UserId, "UserOption.UserId");
  }

  private TestGlobalOption CreateTestGlobalOption(
    string name, object? defaultValue = null) {
    return new TestGlobalOption(QueryHelper, Session, name, defaultValue);
  }
}