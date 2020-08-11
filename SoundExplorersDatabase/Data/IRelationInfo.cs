using System;

namespace SoundExplorersDatabase.Data {
  public interface IRelationInfo {
    Type ParentType { get; }
    bool IsMandatory { get; }
  }
}