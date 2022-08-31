using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  /// <summary>
  ///   Accesses a global (not user-specific) option/preference, as held on the
  ///   UserOption table.
  /// </summary>
  public class GlobalOption : Option {
    public GlobalOption(QueryHelper queryHelper, SessionBase session,
      string name, object? defaultValue = null) : base(
      queryHelper, session, name, defaultValue) { }

    /// <summary>
    ///   Gets the global id, which indicates that the option is not
    ///   user-specific. 
    /// </summary>
    /// <remarks>
    ///   To minimise the possibility that the global id will coincide with a real user
    ///   id, it starts with a character than can be read but not typed. 
    /// </remarks>
    protected override string UserId => "ˉGlobal";
  }
}