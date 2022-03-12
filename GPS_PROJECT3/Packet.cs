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
        public byte[] g = new byte[2];
        public byte[] tail = new byte[2];
        public byte[] hh = new byte[60];
        private bool isReal;
        private ushort crcCheckerValue;

        static ushort[] crc_table = new ushort[256] { 0x0000, 0x9705, 0x2E01, 0xB904, 0x5C02, 0xCB07, 0x7203, 0xE506, 0xB804, 0x2F01, 0x9605, 0x0100, 0xE406, 0x7303, 0xCA07, 0x5D02, 0x7003, 0xE706, 0x5E02, 0xC907, 0x2C01, 0xBB04, 0x0200, 0x9505, 0xC807, 0x5F02, 0xE606, 0x7103, 0x9405, 0x0300, 0xBA04, 0x2D01, 0xE006, 0x7703, 0xCE07, 0x5902, 0xBC04, 0x2B01, 0x9205, 0x0500, 0x5802, 0xCF07, 0x7603, 0xE106, 0x0400, 0x9305, 0x2A01, 0xBD04, 0x9005, 0x0700, 0xBE04, 0x2901, 0xCC07, 0x5B02, 0xE206, 0x7503, 0x2801, 0xBF04, 0x0600, 0x9105, 0x7403, 0xE306, 0x5A02, 0xCD07, 0xC007, 0x5702, 0xEE06, 0x7903, 0x9C05, 0x0B00, 0xB204, 0x2501, 0x7803, 0xEF06, 0x5602, 0xC107, 0x2401, 0xB304, 0x0A00, 0x9D05, 0xB004, 0x2701, 0x9E05, 0x0900, 0xEC06, 0x7B03, 0xC207, 0x5502, 0x0800, 0x9F05, 0x2601, 0xB104, 0x5402, 0xC307, 0x7A03, 0xED06, 0x2001, 0xB704, 0x0E00, 0x9905, 0x7C03, 0xEB06, 0x5202, 0xC507, 0x9805, 0x0F00, 0xB604, 0x2101, 0xC407, 0x5302, 0xEA06, 0x7D03, 0x5002, 0xC707, 0x7E03, 0xE906, 0x0C00, 0x9B05, 0x2201, 0xB504, 0xE806, 0x7F03, 0xC607, 0x5102, 0xB404, 0x2301, 0x9A05, 0x0D00, 0x8005, 0x1700, 0xAE04, 0x3901, 0xDC07, 0x4B02, 0xF206, 0x6503, 0x3801, 0xAF04, 0x1600, 0x8105, 0x6403, 0xF306, 0x4A02, 0xDD07, 0xF006, 0x6703, 0xDE07, 0x4902, 0xAC04, 0x3B01, 0x8205, 0x1500, 0x4802, 0xDF07, 0x6603, 0xF106, 0x1400, 0x8305, 0x3A01, 0xAD04, 0x6003, 0xF706, 0x4E02, 0xD907, 0x3C01, 0xAB04, 0x1200, 0x8505, 0xD807, 0x4F02, 0xF606, 0x6103, 0x8405, 0x1300, 0xAA04, 0x3D01, 0x1000, 0x8705, 0x3E01, 0xA904, 0x4C02, 0xDB07, 0x6203, 0xF506, 0xA804, 0x3F01, 0x8605, 0x1100, 0xF406, 0x6303, 0xDA07, 0x4D02, 0x4002, 0xD707, 0x6E03, 0xF906, 0x1C00, 0x8B05, 0x3201, 0xA504, 0xF806, 0x6F03, 0xD607, 0x4102, 0xA404, 0x3301, 0x8A05, 0x1D00, 0x3001, 0xA704, 0x1E00, 0x8905, 0x6C03, 0xFB06, 0x4202, 0xD507, 0x8805, 0x1F00, 0xA604, 0x3101, 0xD407, 0x4302, 0xFA06, 0x6D03, 0xA004, 0x3701, 0x8E05, 0x1900, 0xFC06, 0x6B03, 0xD207, 0x4502, 0x1800, 0x8F05, 0x3601, 0xA104, 0x4402, 0xD307, 0x6A03, 0xFD06, 0xD007, 0x4702, 0xFE06, 0x6903, 0x8C05, 0x1B00, 0xA204, 0x3501, 0x6803, 0xFF06, 0x4602, 0xD107, 0x3401, 0xA304, 0x1A00, 0x8D05 };

        public Packet(byte[] rawPacket)
        {
            Header = rawPacket[0..2];
            Length = BitConverter.ToUInt16(rawPacket[2..4]);
            UNIT_code = rawPacket[4..16];
            Event_code = rawPacket[16..18];
            tail = rawPacket[^2..^0];
            crcCheckerValue = BitConverter.ToUInt16(rawPacket[^4..^2]);
            this.checker(rawPacket);
        }
        private void checker(byte[] rawPacket)
        {
            // Толгой болон сүүл нь таарч байна уу гэдгийг шалгаж байна. 64 = 40 ; 13 = D; 10 = A;
            if(Header[0] == 64 && Header[1] == 64 && tail[0] == 13 && tail[1] == 10)
            {
                // PACKET-с уншсан уртын мэдээллийг ашиглан 1024 байтаас илүүгүй байгаа эсэхийг шалгаж байна. 
                if(Length <= 1024)
                {
                    // Дээрх нөхцөлүүдийг хангасан тохиолдолд CRC-г тооцоолж packet -н мэдээлэл дунд байгаа тоотой тэнцүү байгаа эсэхийг шалгаж байна.
                    if(crcCheckerValue == CRC16(rawPacket))
                    {
                        this.isReal = true;
                        Console.WriteLine(true);
                    }else
                    {
                        this.isReal = false;
                        //Console.WriteLine(false);
                        //Console.WriteLine(crcCheckerValue);
                    }

                }else
                {
                    this.isReal = false; 
                }
                
            } else
            {
                this.isReal = false;
                //Console.WriteLine(false);
            }
        }

        // DECIMAL TO HEX
        private void byteToASCII(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                Console.WriteLine(b.ToString("X"));
            }
        }

        // Үүнд тайлан дээр байсан кодыг хэрэгжүүлсэн. 16222 гэсэн хариу гарч байсан дээрх тохиолдолд.
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

        // CRC CALCULATOR Багшийн өгсөн аргыг хэрэгжүүлэв. 12146 гэсэн хариу дээрх тохиолдолд гарч байсан.
        internal static ushort CRC16(byte[] rawData)
        {
            ushort crc = 0xffff;
            ushort size = (ushort)rawData.Length;

            for (uint i = 0; i < size; i++)
            {
                crc = (ushort)(crc_table[(crc ^ rawData[i]) & 0xff] ^ (crc >> 8));
            }

            return crc;
        }

        public bool getIsRealValue()
        {
            return this.isReal;
        }
    }
}
