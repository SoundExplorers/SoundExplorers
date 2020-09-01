using System.Collections.Generic;
using SoundExplorers.Common;

namespace SoundExplorers.Data {
  internal class EntityColumnComparer : Comparer<IEntityColumn> {
    public override int Compare(IEntityColumn x, IEntityColumn y) {
      if (x?.SequenceNo < y?.SequenceNo) {
        return -1;
      }
      return x?.SequenceNo == y?.SequenceNo ? 0 : 1;
    }
  }
}