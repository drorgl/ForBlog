using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    class unsafeByteCompare : IComparing
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
                    int* aip = (int*)ap, bip = (int*)bp;
                    for (; len >= 4; len -= 4)
                    {
                        if (*aip != *bip) return false;
                        aip++;
                        bip++;
                    }
                    byte* ap2 = (byte*)aip, bp2 = (byte*)bip;
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
