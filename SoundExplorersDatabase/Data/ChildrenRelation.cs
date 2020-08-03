using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenRelation {
    public ChildrenRelation(
      [NotNull] Type childType,
      [NotNull] ISortedChildList children) {
      ChildType = childType ??
                  throw new ArgumentNullException(nameof(childType));
      Children = children ??
                 throw new ArgumentNullException(nameof(childType));
    }

    public Type ChildType { get; }
    public ISortedChildList Children { get; }
  }
}