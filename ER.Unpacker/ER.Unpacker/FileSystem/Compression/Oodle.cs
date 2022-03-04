using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ER.Unpacker
{
    class Oodle
    {
        [DllImport("oo2core_6_win64.dll", EntryPoint = "OodleLZ_Decompress")]
        private static extern Int64 OodleLZ_Decompress(Byte[] lpScrBuffer, Int32 dwCompressedSize, Byte[] lpDstBuffer, Int32 dwDecompressedSize, UInt32 a, UInt32 b, UInt64 c, UInt32 d, UInt32 e, UInt32 f, UInt32 g, UInt32 h, UInt32 i, UInt32 threadModule);

        public static Byte[] iDecompress(Byte[] lpScrBuffer, Int32 dwCompressedSize, Int32 dwDecompressedSize)
        {
            Byte[] lpDstBuffer = new Byte[dwDecompressedSize];
            Int64 dwResult = OodleLZ_Decompress(lpScrBuffer, dwCompressedSize, lpDstBuffer, dwDecompressedSize, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3);

            if (dwResult == dwDecompressedSize)
            {
                return lpDstBuffer;
            }
            else if (dwResult < dwDecompressedSize)
            {
                return lpDstBuffer.Take((Int32)dwResult).ToArray();
            }
            else
            {
                throw new Exception("[ERROR]: There was an error while decompressing data!");
            }
        }
    }
}
