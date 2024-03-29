﻿using NUnit.Framework;
using SoundExplorers.Data;

namespace SoundExplorers.Tests.Data; 

public abstract class TestFixtureBase {
  protected QueryHelper QueryHelper { get; private set; } = null!;
  protected TestData Data { get; private set; } = null!;
  protected TestSession Session { get; private set; } = null!;
  private string DatabaseFolderPath { get; set; } = null!;

  [SetUp]
  public virtual void Setup() {
    QueryHelper = new QueryHelper();
    Data = new TestData(QueryHelper);
    DatabaseFolderPath = TestSession.CreateDatabaseFolder();
    Session = new TestSession(DatabaseFolderPath);
  }

  [TearDown]
  public virtual void TearDown() {
    TestSession.DeleteFolderIfExists(DatabaseFolderPath);
  }
}