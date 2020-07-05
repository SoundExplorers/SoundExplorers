using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundExplorers.Data {

    internal class DeepCloneable<T> : ICloneable {

        #region Public Methods
        T Clone() {
            return (T)MemberwiseClone();
        }
        #endregion Public Methods

        #region ICloneable Members
        object ICloneable.Clone() {
            return this.Clone();
        }
        #endregion
    }
}
