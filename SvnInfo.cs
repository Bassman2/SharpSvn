namespace SharpSvn
{
    public class SvnInfo
    {
        public SvnNodeKind NodeKind { get; internal set; }
        public long LastChangeRevision { get; internal set; }
        public long RepositorySize { get; internal set; }
        public bool Conflicted { get; internal set; }
        public string ChangeList { get; internal set; }
    }
}
