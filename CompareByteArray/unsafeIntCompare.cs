using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    class unsafeIntCompare : IComparing
    {
        #region IComparing Members

        //http://stackoverflow.com/questions/1389570/c-sharp-byte-array-comparison-issue
        public unsafe bool Compare(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            int len = b1.Length;
            fixed (byte* p1 = b1, p2 = b2)
            {
                int* i1 = (int*)p1;
                int* i2 = (int*)p2;
                while (len >= 4)
                {
                    if (*i1 != *i2) return false;
                    i1++;
                    i2++;
                    len -= 4;
                }
                byte* c1 = (byte*)i1;
                byte* c2 = (byte*)i2;
                while (len > 0)
                {
                    if (*c1 != *c2) return false;
                    c1++;
                    c2++;
                    len--;
                }
            }
            return true;
        }

        #endregion
    }
}
