using System;
using System.Collections.Generic;

namespace ER.Unpacker
{
    class BinderEntry
    {
        public UInt64 dwNameHash { get; set; }
        public Int32 dwPaddedSize { get; set; }
        public Int32 dwSize { get; set; }
        public Int64 dwOffset { get; set; }
        public Int64 dwShaHashOffset { get; set; }
        public Int64 dwAesKeyOffset { get; set; }
        public Byte[] lpShaHash { get; set; }
        public Byte[] lpAesKey { get; set; }
        public BinderRangeEntry[] m_RangeTable { get; set; }
    }
}
