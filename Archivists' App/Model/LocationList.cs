using System.Collections;
using System.Linq;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class LocationList : NotablyNamedEntityList<Location> {
    public override IList GetChildren(int rowIndex) {
      return this[rowIndex].Events.Values.ToList();
    }
  }
}