using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpSvn
{
    public sealed class SvnClient : IDisposable
    {
        private SvnClient17 svnClient17;
        private SvnClient18 svnClient18;
        private SvnClient19 svnClient19;

        public SvnClient()
        {
            this.svnClient17 = new SvnClient17();
            this.svnClient18 = new SvnClient18();
            this.svnClient19 = new SvnClient19();
        }

        public SvnClient(string login, string password)
        {
            this.svnClient17 = new SvnClient17();
            this.svnClient17.Authenticate(login, password);

            this.svnClient18 = new SvnClient18();
            this.svnClient18.Authenticate(login, password);

            this.svnClient19 = new SvnClient19();
            this.svnClient19.Authenticate(login, password);
        }

        public void Dispose()
        {
            if (this.svnClient17 != null)
            {
                svnClient17.Dispose();
                this.svnClient17 = null;
            }
            if (this.svnClient18 != null)
            {
                svnClient18.Dispose();
                this.svnClient18 = null;
            }
            if (this.svnClient19 != null)
            {
                svnClient19.Dispose();
                this.svnClient19 = null;
            }
        }

        internal object Handle
        {
            get
            {
                return this.svnClient18.Handle;
            }
        }

        public void Authenticate(string login, string password)
        {
            try
            {
                this.svnClient17.Authenticate(login, password);
                this.svnClient18.Authenticate(login, password);
                this.svnClient19.Authenticate(login, password);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Get subversion client version with this path is checked out.
        /// </summary>
        /// <param name="path">Checkout path</param>
        /// <returns></returns>
        public SvnVersion GetVersion(string path)
        {
            if (this.svnClient17.GetUriFromWorkingCopy(path) != null)
            {
                return SvnVersion.Version17;
            }
            if (this.svnClient18.GetUriFromWorkingCopy(path) != null)
            {
                return SvnVersion.Version18;
            }
            if (this.svnClient19.GetUriFromWorkingCopy(path) != null)
            {
                return SvnVersion.Version19;
            }
            return SvnVersion.Unknown;
        }

        public Uri GetUriFromWorkingCopy(string path)
        {
            Uri uri = null;
            if ((uri = this.svnClient17.GetUriFromWorkingCopy(path)) != null)
            {
                return uri;
            }
            if ((uri = this.svnClient18.GetUriFromWorkingCopy(path)) != null)
            {
                return uri;
            }
            if ((uri = this.svnClient19.GetUriFromWorkingCopy(path)) != null)
            {
                return uri;
            }
            return null;
        }
        
        public SvnInfo GetInfo(string path)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.GetInfo(path);
            case SvnVersion.Version18:
                return this.svnClient18.GetInfo(path);
            case SvnVersion.Version19:
                return this.svnClient19.GetInfo(path);
            default:
                return null;
            }
        }


        public SvnInfo GetInfo(Uri uri)
        {
            return this.svnClient18.GetInfo(uri);
        }

        public SvnInfo GetInfo(Uri uri, long revision)
        {
            return this.svnClient18.GetInfo(uri, revision);
        }

        public IEnumerable<SvnStatus> GetStatus(string path)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.GetStatus(path);
            case SvnVersion.Version18:
                return this.svnClient18.GetStatus(path);
            case SvnVersion.Version19:
                return this.svnClient19.GetStatus(path);
            default:
                return null;
            }
        }

        public IEnumerable<SvnList> GetList(string path)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.GetList(path);
            case SvnVersion.Version18:
                return this.svnClient18.GetList(path);
            case SvnVersion.Version19:
                return this.svnClient19.GetList(path);
            default:
                return null;
            }
        }

        public IEnumerable<SvnList> GetList(Uri uri)
        {
            return this.svnClient18.GetList(uri);
        }

        public IEnumerable<SvnList> GetList(Uri uri, long revision)
        {
            return this.svnClient19.GetList(uri, revision);
        }

        public bool Write(string path, Stream stream)
        {
            return this.svnClient18.Write(path, stream);
        }

        public bool Write(Uri uri, Stream stream)
        {
            return this.svnClient18.Write(uri, stream);
        }

        public bool Write(Uri uri, long revision, Stream stream)
        {
            return this.svnClient18.Write(uri, revision, stream);
        }
        
        public bool Update(string path)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.Update(path);
            case SvnVersion.Version18:
                return this.svnClient18.Update(path);
            case SvnVersion.Version19:
                return this.svnClient19.Update(path);
            default:
                return false;
            }
        }

        public bool Commit(string path)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.Commit(path);
            case SvnVersion.Version18:
                return this.svnClient18.Commit(path);
            case SvnVersion.Version19:
                return this.svnClient19.Commit(path);
            default:
                return false;
            }
        }  
      
        public bool Commit(string path, string comment)
        {
            switch (GetVersion(path))
            {
            case SvnVersion.Version17:
                return this.svnClient17.Commit(path, comment);
            case SvnVersion.Version18:
                return this.svnClient18.Commit(path, comment);
            case SvnVersion.Version19:
                return this.svnClient19.Commit(path, comment);
            default:
                return false;
            }
        }  

        public bool TryGetRepositoryId(Uri uri, out Guid id)
        {
            return this.svnClient18.TryGetRepositoryId(uri, out id);
        }


        public static bool TryGetTarget(string target, out Uri uri, out long revision)
        {
            revision = -1;
            uri = null;
            try
            {
                string[] par = target.Split(new char[] { '@' }, 2);
                uri = new Uri(par[0]);
                if (par.Length > 1)
                {
                    revision = long.Parse(par[1]);
                }
                return true;
            }
            catch 
            {}
            return false;
        }

    }
}
