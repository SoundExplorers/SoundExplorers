using System;

namespace SoundExplorersDatabase {
  public interface IRelationInfo {
    bool IsMandatory { get; }
    Type ParentType { get; }
  }
}