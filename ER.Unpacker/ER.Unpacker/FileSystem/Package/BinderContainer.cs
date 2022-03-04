using System;
using System.IO;

namespace ER.Unpacker
{
    class BinderContainer
    {
        public static void iDecompressFile(Byte[] lpBuffer, String m_File)
        {
            using (var TMemoryStream = new MemoryStream(lpBuffer))
            {
                UInt32 dwDCXMagic = TMemoryStream.ReadUInt32(); // DCX\0 (0x584344)
                if (dwDCXMagic != 0x584344)
                {
                    throw new Exception("[ERROR]: Invalid magic of DCX file!");
                }

                Int32 dwChunkSize = TMemoryStream.ReadInt32(true); // 65536

                Int32 dwDCSOffset = TMemoryStream.ReadInt32(true); // 24
                Int32 dwDCPOffset = TMemoryStream.ReadInt32(true); // 36
                Int32 dwDCAOffset = TMemoryStream.ReadInt32(true); // 68
                Int32 dwCMPOffset = TMemoryStream.ReadInt32(true); // 76

                UInt32 dwDCSMagic = TMemoryStream.ReadUInt32(); // DCS\0 (0x534344)
                if (dwDCSMagic != 0x534344)
                {
                    throw new Exception("[ERROR]: Invalid DCS magic in file!");
                }

                Int32 dwDecompressedSize = TMemoryStream.ReadInt32(true);
                Int32 dwCompressedSize = TMemoryStream.ReadInt32(true);

                UInt32 dwDCPMagic = TMemoryStream.ReadUInt32(); // DCP\0 (0x504344)
                if (dwDCPMagic != 0x504344)
                {
                    throw new Exception("[ERROR]: Invalid DCP magic in file!");
                }

                UInt32 dwCMPMagic = TMemoryStream.ReadUInt32(); // DFLT (Zlib), KRAK (Oodle) (0x44464C54, 0x4B41524B)
                var lpProps = TMemoryStream.ReadBytes(24); // Including header size & compression level

                UInt32 dwDCAMagic = TMemoryStream.ReadUInt32(); // DCA\0 (0x414344)
                if (dwDCAMagic != 0x414344)
                {
                    throw new Exception("[ERROR]: Invalid DCA magic in file!");
                }

                Int32 dwDCAHeaderSize = TMemoryStream.ReadInt32(true); // 8

                var lpScrBuffer = TMemoryStream.ReadBytes(dwCompressedSize);
                TMemoryStream.Dispose();

                String m_FullPath = m_File.Replace(".dcx", "");
                if (dwCMPMagic == 0x544C4644)
                {
                    var lpDstBuffer = Zlib.iDecompress(lpScrBuffer);
                    File.WriteAllBytes(m_FullPath, lpDstBuffer);
                }
                else if (dwCMPMagic == 0x4B41524B)
                {
                    var lpDstBuffer = Oodle.iDecompress(lpScrBuffer, dwCompressedSize, dwDecompressedSize);
                    File.WriteAllBytes(m_FullPath, lpDstBuffer);
                }
                else
                {
                    throw new Exception("[ERROR]: Unsupported compression type!");
                }
            }
        }
    }
}
