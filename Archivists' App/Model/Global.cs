using SoundExplorers.Data;
using VelocityDb.Session;

namespace SoundExplorers.Model {
  public static class Global {
    public static QueryHelper QueryHelper { get; set; }
    public static SessionBase Session { get; set; }
  }
}