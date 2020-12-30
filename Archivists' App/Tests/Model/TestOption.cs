using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model {
  public class TestOption : Option {
    public TestOption(QueryHelper queryHelper, SessionBase session,
      string name, object? defaultValue = null) : base(queryHelper, session,
      name, defaultValue) { }
  }
}