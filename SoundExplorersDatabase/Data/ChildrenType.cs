using System;
using JetBrains.Annotations;

namespace SoundExplorersDatabase.Data {
  public class ChildrenType {
    public ChildrenType(
      [NotNull] Type childType,
      [NotNull] ISortedChildList children) {
      ChildType = childType;
      Children = children;
    }

    public Type ChildType { get; }
    public ISortedChildList Children { get; }
  }
}