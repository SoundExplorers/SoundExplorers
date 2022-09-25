using System;

namespace SoundExplorers.Data; 

public interface IRelationInfo {
  bool IsMandatory { get; }
  Type ParentType { get; }
}