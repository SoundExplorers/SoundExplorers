﻿using System;
using NUnit.Framework;
using SoundExplorers.Data;
using SoundExplorers.Tests.Data;

namespace SoundExplorers.Tests.Model {
  [TestFixture]
  public class OptionTests {
    [SetUp]
    public void Setup() {
      QueryHelper = new QueryHelper();
      Session = new TestSession();
      Session.BeginUpdate();
      Session.Commit();
    }

    [TearDown]
    public void TearDown() {
      Session.DeleteDatabaseFolderIfExists();
    }

    private QueryHelper QueryHelper { get; set; } = null!;
    private TestSession Session { get; set; } = null!;

    [Test]
    public void DefaultsDefaulted() {
      var option = CreateTestOption("Boolean");
      Assert.IsFalse(option.BooleanValue, "BooleanValue default");
      option.BooleanValue = true;
      option = CreateTestOption("Boolean");
      Assert.IsTrue(option.BooleanValue, "BooleanValue retrieved");
      option = CreateTestOption("Int32");
      Assert.AreEqual(0, option.Int32Value, "Int32Value default");
      option.Int32Value = 1;
      option = CreateTestOption("Int32");
      Assert.AreEqual(1, option.Int32Value, "Int32Value retrieved");
      option = CreateTestOption("String");
      string simpleKey = $"{Environment.UserName}|String";
      Session.BeginRead();
      Assert.IsNull(
        QueryHelper.Find<UserOption>(simpleKey, Session),
        "UserOption 'String' unpersisted initially");
      Session.Commit();
      Assert.IsEmpty(option.StringValue, "StringValue default");
      option.StringValue = "Hello";
      Session.BeginRead();
      Assert.IsNotNull(
        QueryHelper.Find<UserOption>(simpleKey, Session),
        "UserOption 'String' persisted when changed to non-default value");
      Session.Commit();
      option = CreateTestOption("String");
      Assert.AreEqual("Hello", option.StringValue, "StringValue retrieved");
      option.StringValue = string.Empty; // Equivalent to default, so needs to deleted
      Session.BeginRead();
      Assert.IsNull(
        QueryHelper.Find<UserOption>(simpleKey, Session),
        "UserOption 'String' unpersisted when reset to default");
      Session.Commit();
    }

    [Test]
    public void DefaultsSet() {
      var option = CreateTestOption("Boolean", true);
      Assert.IsTrue(option.BooleanValue, "BooleanValue");
      option = CreateTestOption("Int32", 1);
      Assert.AreEqual(1, option.Int32Value, "Int32Value");
      option = CreateTestOption("String", "Hello");
      Assert.AreEqual("Hello", option.StringValue, "StringValue");
      // I can't think how to test this
      // without making an otherwise unnecessary change to the tested code,
      // apart from noting that it adds to code coverage.
      option.StringValue = "Hello"; // Unchanged, so does not need to be saved
    }

    private TestOption CreateTestOption(
      string name, object? defaultValue = null) {
      return new TestOption(QueryHelper, Session, name, defaultValue);
    }
  }
}