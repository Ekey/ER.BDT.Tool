using System;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace ER.Unpacker
{
    class BinderCipher
    {
        private static String iGetRSABinderKey(String m_BinderFile)
        {
            m_BinderFile = Path.GetFileNameWithoutExtension(m_BinderFile);
            switch (m_BinderFile)
            {
                case "Data0": return BinderKeys.m_Data0;
                case "Data1": return BinderKeys.m_Data1;
                case "Data2": return BinderKeys.m_Data2;
                case "Data3": return BinderKeys.m_Data3;
                case "sd": return BinderKeys.m_Sd;
                default: return "null";
            }
        }

        //Part code from BinderTool project by Atvaark
        //https://github.com/Atvaark/BinderTool
        //TODO: Rewrite the code and remove the BouncyCastle dependency
        public static MemoryStream iDecryptByRSA(String m_BinderFile)
        {
            AsymmetricKeyParameter TKeyPair = (AsymmetricKeyParameter)new PemReader(new StringReader(iGetRSABinderKey(m_BinderFile))).ReadObject();
            RsaEngine TRsaEngine = new RsaEngine();
            TRsaEngine.Init(false, TKeyPair);

            MemoryStream TMemoryStream = new MemoryStream();
            using (FileStream TFileStream = File.OpenRead(m_BinderFile))
            {
                Int32 dwScrBlockSize = TRsaEngine.GetInputBlockSize();
                Int32 dwDstBlockSize = TRsaEngine.GetOutputBlockSize();
                Byte[] lpSrcBlock = new Byte[dwScrBlockSize];
                while (TFileStream.Read(lpSrcBlock, 0, lpSrcBlock.Length) > 0)
                {
                    Byte[] lpDstBlock = TRsaEngine.ProcessBlock(lpSrcBlock, 0, dwScrBlockSize);
                    Int32 requiredPadding = dwDstBlockSize - lpDstBlock.Length;
                    if (requiredPadding > 0)
                    {
                        Byte[] lpDstPaddedBlock = new Byte[dwDstBlockSize];
                        lpDstBlock.CopyTo(lpDstPaddedBlock, requiredPadding);
                        lpDstBlock = lpDstPaddedBlock;
                    }
                    TMemoryStream.Write(lpDstBlock, 0, lpDstBlock.Length);
                }
            }
            TMemoryStream.Seek(0L, SeekOrigin.Begin);

            return TMemoryStream;
        }

        //Part code from SoulsFormats project by JKAnderson
        //https://github.com/JKAnderson/SoulsFormats
        public static Byte[] iDecryptByAES(Byte[] lpBuffer, BinderEntry m_Entry)
        {
            RijndaelManaged TAES = new RijndaelManaged();

            TAES.KeySize = 128;
            TAES.BlockSize = 128;
            TAES.Key = m_Entry.lpAesKey;
            TAES.IV = new Byte[16];
            TAES.Mode = CipherMode.ECB;
            TAES.Padding = PaddingMode.None;

            foreach (var m_Range in m_Entry.m_RangeTable)
            {
                if (m_Range.dwStartOffset != -1 && m_Range.dwEndOffset != -1 && m_Range.dwStartOffset != m_Range.dwEndOffset)
                {
                    ICryptoTransform TICryptoTransform = TAES.CreateDecryptor();
                    TICryptoTransform.TransformBlock(lpBuffer, (Int32)m_Range.dwStartOffset, (Int32)(m_Range.dwEndOffset - m_Range.dwStartOffset), lpBuffer, (Int32)m_Range.dwStartOffset);
                }
            }

            return lpBuffer;
        }
    }
}
