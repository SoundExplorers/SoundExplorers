using JetBrains.Annotations;
using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model {
  public class TestOption : Option {
    public TestOption([NotNull] QueryHelper queryHelper, [NotNull] SessionBase session,
      [NotNull] string name, object defaultValue = null) : base(queryHelper, session,
      name, defaultValue) { }
  }
}