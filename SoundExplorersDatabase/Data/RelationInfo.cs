using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class RelationInfo : IRelationInfo {
    internal RelationInfo(
      [NotNull] Type parentType, [NotNull] Type childType, bool isMandatory) {
      ParentType = parentType ??
                   throw new ArgumentNullException(nameof(parentType));
      ChildType = childType ??
                  throw new ArgumentNullException(nameof(childType));
      IsMandatory = isMandatory;
    }

    [NotNull] public Type ChildType { get; }
    public bool IsMandatory { get; }
    [NotNull] public Type ParentType { get; }
  }
}