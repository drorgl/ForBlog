using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    class SimpleCompare : IComparing
    {
        //http://stackoverflow.com/questions/43289/comparing-two-byte-arrays-in-net
        public bool Compare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }
    }
}
