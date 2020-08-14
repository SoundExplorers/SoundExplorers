using System;

namespace SoundExplorersDatabase.Data {
  public interface IRelative {
    RelativeBase IdentifyingParent { get; }
    Key Key { get; }
    string SimpleKey { get; }
  }
}