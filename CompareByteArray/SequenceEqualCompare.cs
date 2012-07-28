using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    class SequenceEqualCompare : IComparing
    {
        #region IComparing Members

        public bool Compare(byte[] array1, byte[] array2)
        {
            return Enumerable.SequenceEqual(array1, array2);
        }

        #endregion
    }
}
