using System;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Common;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class LocationList : NotablyNamedEntityList<Location> {
    [ExcludeFromCodeCoverage]
    public override IdentifyingParentChildren GetIdentifyingParentChildrenForMainList(
      int rowIndex) {
      throw new NotSupportedException();
    }
  }
}