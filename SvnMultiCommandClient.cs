using System;
using System.IO;

namespace SharpSvn
{
    public class SvnMultiCommandClient : IDisposable
    {
        private SvnMultiCommandClient18 svnMultiCommandClient18;

        public SvnMultiCommandClient()
        {
            this.svnMultiCommandClient18 = new SvnMultiCommandClient18();
        }

        public SvnMultiCommandClient(string login, string password)
        {
            this.svnMultiCommandClient18 = new SvnMultiCommandClient18(login, password);
        }


        public void Dispose()
        {
            if (this.svnMultiCommandClient18 != null)
            {
                svnMultiCommandClient18.Dispose();
                this.svnMultiCommandClient18 = null;
            }
        }

        public bool Open(Uri sessionUri)
        {
            return this.svnMultiCommandClient18.Open(sessionUri);
        }

        public bool FilesExists(string path)
        {
            return this.svnMultiCommandClient18.FilesExists(path);
        }

        public bool CreateFile(string path, Stream stream)
        {
            return this.svnMultiCommandClient18.CreateFile(path, stream);
        }

        public bool UpdateFile(string path, Stream stream)
        {
            return this.svnMultiCommandClient18.UpdateFile(path, stream);
        }

        public bool Delete(string path)
        {
            return this.svnMultiCommandClient18.Delete(path);
        }

        public bool SetProperty(string path, string propertyName, string value)
        {
            return this.svnMultiCommandClient18.SetProperty(path, propertyName, value);
        }

        public bool Commit()
        {
            return this.svnMultiCommandClient18.Commit();
        }

        public bool Commit(string logMessage)
        {
            return this.svnMultiCommandClient18.Commit(logMessage);
        }
    }
}
