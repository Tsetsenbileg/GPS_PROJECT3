using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_PROJECT3
{
    class Packet
    {
        public byte[] Header = new byte[2];
        public ushort Length = 0;
        public byte[] UNIT_code = new byte[12];
        public byte[] Event_code = new byte[2];
        public byte[] Event_data = new byte[2];
        public ushort crc_sum;
        public byte[] g = new byte[2];
        public byte[] tail = new byte[2];
        public byte[] hh = new byte[60];

        public Packet(byte[] rawPacket)
        {

            foreach (byte b in rawPacket)
            {
                Console.WriteLine(b);
            };
            Header = rawPacket[0..2];
            Length = BitConverter.ToUInt16(rawPacket[2..4]);
            UNIT_code = rawPacket[4..16];
            Event_code = rawPacket[16..18];
            crc_sum = crc16(rawPacket);
            tail = rawPacket[^2..^1];
        }
        public void ShowHead()
        {
            foreach (byte ddd in UNIT_code)
            {
                Console.WriteLine(ddd);
            }
        }

        public void byteToASCII()
        {
            //foreach (byte b in UNIT_code)
            //{
            //    Console.WriteLine(b);
            //}
            Console.WriteLine(Length);
        }

        private ushort crc16(byte[] bytes)
        {
            const ushort poly = 4129;
            ushort[] table = new ushort[256];
            ushort initialValue = 0xffff;
            ushort temp, a;
            ushort crc = initialValue;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                        temp = (ushort)((temp << 1) ^ poly);
                    else
                        temp <<= 1;
                    a <<= 1;
                }
                table[i] = temp;
            }
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
    }
}
