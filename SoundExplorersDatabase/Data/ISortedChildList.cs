using System.Collections;

namespace SoundExplorersDatabase.Data {
  public interface ISortedChildList : IDictionary {
    bool IsMembershipMandatory { get; }
  }
}