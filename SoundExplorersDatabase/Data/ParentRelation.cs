using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ParentRelation {
    public ParentRelation([NotNull] Type parentType, bool isMandatory) {
      ParentType = parentType ??
                   throw new ArgumentNullException(nameof(parentType));
      IsMandatory = isMandatory;
    }

    [NotNull] public Type ParentType { get; }
    public bool IsMandatory { get; }
  }
}