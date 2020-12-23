﻿using JetBrains.Annotations;
using SoundExplorers.Controller;
using SoundExplorers.Model;
using SoundExplorers.Tests.Model;

namespace SoundExplorers.Tests.Controller {
  public class TestOptionsController : OptionsController {
    public TestOptionsController([NotNull] IView<OptionsController> view,
      [NotNull] MockDatabaseConfig mockDatabaseConfig) : base(view) {
      MockDatabaseConfig = mockDatabaseConfig;
    }

    private MockDatabaseConfig MockDatabaseConfig { get; }

    protected override IDatabaseConfig CreateDatabaseConfig() {
      return MockDatabaseConfig;
    }

    protected override string GetProductName() {
      return "Sound Explorers Audio Archive";
    }
  }
}