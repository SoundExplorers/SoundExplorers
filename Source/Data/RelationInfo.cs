using System;

namespace SoundExplorers.Data; 

public class RelationInfo : IRelationInfo {
  internal RelationInfo(
    Type parentType, Type childType, bool isMandatory) {
    ParentType = parentType ??
                 throw new ArgumentNullException(nameof(parentType));
    ChildType = childType ??
                throw new ArgumentNullException(nameof(childType));
    IsMandatory = isMandatory;
  }

  public Type ChildType { get; }
  public bool IsMandatory { get; }
  public Type ParentType { get; }
}