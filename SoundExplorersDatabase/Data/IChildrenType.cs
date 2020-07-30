using System;
using System.Collections;

namespace SoundExplorersDatabase.Data {
  public interface IChildrenType {
    Type ChildType { get; }
    IDictionary Children { get; }
  }
}