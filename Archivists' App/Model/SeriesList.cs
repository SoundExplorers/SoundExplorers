using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SoundExplorers.Data;

namespace SoundExplorers.Model {
  public class SeriesList : NotablyNamedEntityList<Series> {
    [ExcludeFromCodeCoverage]
    public override IList GetChildrenForMainList(int rowIndex) {
      throw new NotSupportedException();
    }
  }
}