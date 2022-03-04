using System;

namespace ER.Unpacker
{
    class BinderHeader
    {
        public UInt32 dwMagic { get; set; } // BHD5 (0x35444842)
        public Int32 dwVersion { get; set; } // 511
        public Int32 dwFlags { get; set; } // 1
        public Int32 dwDataSize { get; set; }
        public Int32 dwBucketDirectoryCount { get; set; }
        public Int32 dwBucketDirectoryOffset { get; set; }
        public String m_Salt { get; set; }
    }
}
