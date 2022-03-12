using System;
using System.Linq;

namespace GPS_PROJECT3
{
    class Program
    {
        static void Main(string[] args)
        {

            String dd = "404056003247512d31363031303633380110060a130a2a080f3e40440a7089e616cf005004d03101010505050006000101000018a5a6a7a8a9aaf2adaeafb0b16162636465666768696a6b6c060a130a2a0753e80d0a";
            byte[] gg = StringToByteArray(dd);

            Packet p = new Packet(gg);
            //p.byteToASCII();

        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
