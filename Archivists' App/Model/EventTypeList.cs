using System.Collections;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class EventTypeList : NamedEntityList<EventType> {
    public override IList GetChildren(int rowIndex) {
      return (IList)this[rowIndex].Events.Values;
    }
  }
}