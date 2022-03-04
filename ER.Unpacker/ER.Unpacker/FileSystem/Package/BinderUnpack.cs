using System;
using System.IO;
using System.Collections.Generic;

namespace ER.Unpacker
{
    class BinderUnpack
    {
        static List<BinderEntry> m_EntryTable = new List<BinderEntry>();

        public static void iDoIt(String m_BinderFile, String m_DstFolder)
        {
            BinderHashList.iLoadProject();
            var TMemoryStream = BinderCipher.iDecryptByRSA(m_BinderFile);

            var m_Header = new BinderHeader();
            m_Header.dwMagic = TMemoryStream.ReadUInt32();
            m_Header.dwVersion = TMemoryStream.ReadInt32();
            m_Header.dwFlags = TMemoryStream.ReadInt32();
            m_Header.dwDataSize = TMemoryStream.ReadInt32();
            m_Header.dwBucketDirectoryCount = TMemoryStream.ReadInt32();
            m_Header.dwBucketDirectoryOffset = TMemoryStream.ReadInt32();
            m_Header.m_Salt = TMemoryStream.ReadStringLength();

            if (m_Header.dwMagic != 0x35444842)
            {
                throw new Exception("[ERROR]: Invalid magic of Binder file!");
            }

            if (m_Header.dwVersion != 511)
            {
                throw new Exception("[ERROR]: Invalid version of Binder file!");
            }

            String m_ArchiveFile = m_BinderFile.Replace(".bhd", ".bdt");

            m_EntryTable.Clear();
            for (Int32 i = 0; i < m_Header.dwBucketDirectoryCount; i++)
            {
                Int32 dwBucketEntryCount = TMemoryStream.ReadInt32();
                Int32 dwBucketEntryOffset = TMemoryStream.ReadInt32();
                Int64 dwBucketSavePos = TMemoryStream.Position;

                TMemoryStream.Seek(dwBucketEntryOffset, SeekOrigin.Begin);
                for (Int32 j = 0; j < dwBucketEntryCount; j++)
                {
                    var TEntry = new BinderEntry();
                    TEntry.dwNameHash = TMemoryStream.ReadUInt64();
                    TEntry.dwPaddedSize = TMemoryStream.ReadInt32();
                    TEntry.dwSize = TMemoryStream.ReadInt32();
                    TEntry.dwOffset = TMemoryStream.ReadInt64();
                    TEntry.dwShaHashOffset = TMemoryStream.ReadInt64();
                    TEntry.dwAesKeyOffset = TMemoryStream.ReadInt64();

                    if (TEntry.dwShaHashOffset != 0)
                    {
                        Int64 dwSavePos = TMemoryStream.Position;
                        TMemoryStream.Seek(TEntry.dwShaHashOffset, SeekOrigin.Begin);
                        TEntry.lpShaHash = TMemoryStream.ReadBytes(32);
                        TMemoryStream.Seek(dwSavePos, SeekOrigin.Begin);
                    }

                    if (TEntry.dwAesKeyOffset != 0)
                    {
                        Int64 dwSavePos = TMemoryStream.Position;
                        TMemoryStream.Seek(TEntry.dwAesKeyOffset, SeekOrigin.Begin);
                        TEntry.lpAesKey = TMemoryStream.ReadBytes(16);

                        Int32 dwTotalRanges = TMemoryStream.ReadInt32();
                        BinderRangeEntry[] m_RangeTable = new BinderRangeEntry[dwTotalRanges];

                        for (Int32 k = 0; k < dwTotalRanges; k++)
                        {
                            m_RangeTable[k] = new BinderRangeEntry();
                            m_RangeTable[k].dwStartOffset = TMemoryStream.ReadInt64();
                            m_RangeTable[k].dwEndOffset = TMemoryStream.ReadInt64();
                        }

                        TEntry.m_RangeTable = m_RangeTable;

                        TMemoryStream.Seek(dwSavePos, SeekOrigin.Begin);
                    }

                    m_EntryTable.Add(TEntry);
                }

                TMemoryStream.Seek(dwBucketSavePos, SeekOrigin.Begin);
            }

            if (!File.Exists(m_ArchiveFile))
            {
                Utils.iSetError("[ERROR]: Input archive -> " + m_ArchiveFile + " <- does not exist");
                return;
            }

            using (FileStream TArchiveStream = File.OpenRead(m_ArchiveFile))
            {
                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FileName = BinderHashList.iGetNameFromHashList(m_Entry.dwNameHash);
                    String m_FullPath = m_DstFolder + m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    TArchiveStream.Seek(m_Entry.dwOffset, SeekOrigin.Begin);
                    var lpBuffer = TArchiveStream.ReadBytes(m_Entry.dwPaddedSize);

                    if (m_Entry.lpAesKey != null)
                    {
                        lpBuffer = BinderCipher.iDecryptByAES(lpBuffer, m_Entry);
                    }

                    UInt32 dwMagic = BitConverter.ToUInt32(lpBuffer, 0);
                    if (dwMagic == 0x584344)
                    {
                        BinderContainer.iDecompressFile(lpBuffer, m_FullPath);
                    }
                    else
                    {
                        File.WriteAllBytes(m_FullPath, lpBuffer);
                    }
                }

                TArchiveStream.Dispose();
            }
        }
    }
}
