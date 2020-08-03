using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenRelation {
    public ChildrenRelation(
      [NotNull] Type childType,
      [NotNull] ISortedChildList children) {
      ChildType = childType;
      Children = children;
    }

    public Type ChildType { get; }
    public ISortedChildList Children { get; }
  }
}