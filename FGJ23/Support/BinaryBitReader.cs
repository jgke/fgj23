using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Support
{
    public class BinaryBitReader
    {
        private byte[] Data;
        private int BitPosition;

        public BinaryBitReader(byte[] data)
        {
            Data = data;
            BitPosition = 0;
        }


        public sbyte ReadI8()
        {
            if (BitPosition % 8 == 0)
            {
                var res = (sbyte)Data[BitPosition / 8];
                BitPosition += 8;
                return res;
            }

            int hi = BitPosition / 8;
            int lo = BitPosition / 8 + 1;

            int offset = BitPosition % 8;
            int hiData = (Data[hi] & (255 >> offset)) << offset;
            int loData = Data[lo] >> (8 - offset);

            BitPosition += 8;
            return (sbyte)(hiData | loData);
        }
    }
}
