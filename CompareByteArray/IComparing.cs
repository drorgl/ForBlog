using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompareByteArray
{
    interface IComparing
    {
        bool Compare(byte[] array1, byte[] array2);
    }
}
