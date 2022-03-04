using System;

namespace ER.Unpacker
{
    class BinderHash
    {
        public static UInt64 iGetHash(String m_String)
        {
            UInt64 dwHash = 0;
            for (Int32 i = 0; i < m_String.Length; i++)
            {
                dwHash = m_String[i] + 133 * dwHash;
            }

            return dwHash;
        }
    }
}
