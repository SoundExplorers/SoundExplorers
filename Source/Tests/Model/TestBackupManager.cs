using System;
using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model {
  public class TestBackupManager : BackupManager {
    public TestBackupManager(QueryHelper? queryHelper,
      SessionBase? session) : base(queryHelper, session) {
    }

    internal DateTime TestBackupDateTime { get; set; }

    protected override DateTime GetBackupDateTime() {
      return TestBackupDateTime;
    }
  }
}