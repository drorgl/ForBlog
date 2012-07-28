using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CompareByteArray
{
    class ArrayEqualCompare : IComparing
    {
        #region IComparing Members

        //http://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
        public bool Compare(byte[] array1, byte[] array2)
        {
            return (array1 as IStructuralEquatable).Equals(array2,StructuralComparisons.StructuralEqualityComparer);
        }

        #endregion
    }
}
