using System;

namespace SoundExplorersDatabase.Data {
  public interface IRelationInfo {
    bool IsMandatory { get; }
    Type ParentType { get; }
  }
}