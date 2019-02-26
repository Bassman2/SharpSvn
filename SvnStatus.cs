namespace SharpSvn
{
    public class SvnStatus
    {
        public string FullPath { get; internal set; }
        public SvnState LocalContentStatus { get; internal set; }
        public bool Modified { get; internal set; }
    }
}
