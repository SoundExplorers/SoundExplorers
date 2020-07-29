using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using VelocityDb;

namespace SoundExplorersDatabase.Data {
  public interface IRelativeBase : IReferenceTracked {
    IDictionary<Type, IDictionary> ChildrenOfType { get; }
    IDictionary<Type, bool> IsChangingChildrenOfType { get; }
    IDictionary<Type, bool> IsChangingParentOfType { get; }
    object Key { get; set; }
    IDictionary<Type, IRelativeBase> ParentOfType { get; }
    Type PersistableType { get; }
    bool AddChild([NotNull] IRelativeBase child);
    bool RemoveChild([NotNull] IRelativeBase child);
  }
}