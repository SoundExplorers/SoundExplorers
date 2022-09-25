using SoundExplorers.Data;
using SoundExplorers.Model;
using VelocityDb.Session;

namespace SoundExplorers.Tests.Model; 

public class TestGlobalOption : GlobalOption {
  public TestGlobalOption(QueryHelper queryHelper, SessionBase session,
    string name, object? defaultValue = null) : base(queryHelper, session,
    name, defaultValue) { }

  public new UserOption UserOption => base.UserOption;
}