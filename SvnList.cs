using System;

namespace SharpSvn
{
    public class SvnList
    {
        public string Path { get; set; }
        public SvnNodeKind NodeKind { get; set; }
        public long FileSize { get; set; }
        public Uri Uri { get; set; }
    }
}
