using System;
using System.IO;
using System.IO.Compression;

namespace ER.Unpacker
{
    class Zlib
    {
        public static Byte[] iDecompress(Byte[] lpBuffer)
        {
            var TOutMemoryStream = new MemoryStream();
            using (MemoryStream TMemoryStream = new MemoryStream(lpBuffer) { Position = 2 })
            {
                using (DeflateStream TDeflateStream = new DeflateStream(TMemoryStream, CompressionMode.Decompress, false))
                {
                    TDeflateStream.CopyTo(TOutMemoryStream);
                    TDeflateStream.Dispose();
                }

                TMemoryStream.Dispose();
            }

            return TOutMemoryStream.ToArray();
        }
    }
}
