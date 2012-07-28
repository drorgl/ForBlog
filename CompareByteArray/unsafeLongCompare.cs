using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    class unsafeLongCompare : IComparing
    {
        #region IComparing Members

        //http://stackoverflow.com/questions/2173414/c-sharp-byte-comparison-without-bound-checks
        public unsafe bool Compare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int len = a.Length;
            unsafe
            {
                fixed (byte* ap = a, bp = b)
                {
                    long* alp = (long*)ap, blp = (long*)bp;
                    for (; len >= 8; len -= 8)
                    {
                        if (*alp != *blp) return false;
                        alp++;
                        blp++;
                    }
                    byte* ap2 = (byte*)alp, bp2 = (byte*)blp;
                    for (; len > 0; len--)
                    {
                        if (*ap2 != *bp2) return false;
                        ap2++;
                        bp2++;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
