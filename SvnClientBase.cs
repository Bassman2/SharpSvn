using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpSvn
{
    public class SvnClientBase : IDisposable
    {
        protected Assembly svnAssembly;
        protected Type svnClientType;
        protected Type svnAuthType;
        protected Type svnInfoType;
        protected Type svnStatusType;
        protected Type svnListType;
        internal Type svnTargetType;
        internal Type svnPathTargetType;
        internal Type svnUriTargetType;
        internal Type svnDirEntryType;
        internal Type svnCommitType;
        internal Type svnListArgsType;
        protected object svnClient;

        public SvnClientBase(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            this.svnAssembly = assembly;
            this.svnClientType = this.svnAssembly.GetType("SharpSvn.SvnClient");
            this.svnAuthType = this.svnAssembly.GetType("SharpSvn.Security.SvnAuthentication");
            this.svnInfoType = this.svnAssembly.GetType("SharpSvn.SvnInfoEventArgs");
            this.svnStatusType = this.svnAssembly.GetType("SharpSvn.SvnStatusEventArgs");
            this.svnListType = this.svnAssembly.GetType("SharpSvn.SvnListEventArgs");
            this.svnTargetType = this.svnAssembly.GetType("SharpSvn.SvnTarget");
            this.svnPathTargetType = this.svnAssembly.GetType("SharpSvn.SvnPathTarget");
            this.svnUriTargetType = this.svnAssembly.GetType("SharpSvn.SvnUriTarget");
            this.svnDirEntryType = this.svnAssembly.GetType("SharpSvn.SvnDirEntry");
            this.svnCommitType = this.svnAssembly.GetType("SharpSvn.SvnCommitArgs");
            this.svnListArgsType = this.svnAssembly.GetType("SharpSvn.SvnListArgs");
            if (this.svnAssembly == null || 
                this.svnClientType == null || 
                this.svnAuthType == null || 
                this.svnInfoType == null ||
                this.svnStatusType == null || 
                this.svnListType == null || 
                this.svnTargetType == null || 
                this.svnPathTargetType == null || 
                this.svnUriTargetType == null || 
                this.svnDirEntryType == null ||
                this.svnCommitType == null ||
                this.svnListArgsType == null)
            {
                Trace.TraceError("Initializing SvnClient failed!");
            }
            this.svnClient = Activator.CreateInstance(svnClientType);
        }

        public SvnClientBase(Assembly assembly, string login, string password) : this(assembly)
        {
            Authenticate(login, password);
        }

        public void Dispose()
        {
            if (this.svnClient != null)
            {
                svnClientType.GetMethod("Dispose").Invoke(this.svnClient, null);
                this.svnClient = null;
            }
        }

        public object Handle
        {
            get
            {
                return this.svnClient;
            }
        }

        public virtual void Authenticate(string login, string password)
        {            
            object auth = svnClientType.GetProperty("Authentication").GetValue(this.svnClient);
            svnAuthType.GetMethod("ForceCredentials").Invoke(auth, new object[] { login, password});
        }

        public virtual Uri GetUriFromWorkingCopy(string path)
        {
            var method = svnClientType.GetMethod("GetUriFromWorkingCopy", new Type[] { typeof(string) });
            return (Uri) method.Invoke(this.svnClient, new object[] { path });
        }        
                
        public SvnInfo GetInfo(string path)
        {
            return GetInfo(GetSvnTarget(path));
        }

        public SvnInfo GetInfo(Uri uri, long revision = -1)
        {
            return GetInfo(GetSvnTarget(uri, revision));
        }

        protected SvnInfo GetInfo(object target)
        {
            var param = new object[] { target, null };
            var method = svnClientType.GetMethod("GetInfo", new Type[] { svnTargetType, svnInfoType.MakeByRefType() });
            method.Invoke(this.svnClient, param);
            SvnInfo info = new SvnInfo();
            info.NodeKind = (SvnNodeKind)svnInfoType.GetProperty("NodeKind").GetValue(param[1]);
            info.LastChangeRevision = (long)svnInfoType.GetProperty("LastChangeRevision").GetValue(param[1]);
            info.RepositorySize = (long)svnInfoType.GetProperty("RepositorySize").GetValue(param[1]);
            info.Conflicted = (bool)svnInfoType.GetProperty("Conflicted").GetValue(param[1]);
            info.ChangeList = (string)svnInfoType.GetProperty("ChangeList").GetValue(param[1]);
            return info;
        }

        public IEnumerable<SvnStatus> GetStatus(string path)
        { 
            var param = new object[] { path, null };
            var method = svnClientType.GetMethod("GetStatus", new Type[] { typeof(string), typeof(Collection<>).MakeGenericType(this.svnStatusType).MakeByRefType() });
            method.Invoke(this.svnClient, param);
            ICollection list = (ICollection)param[1];
            return list.Cast<object>().Select(o => new SvnStatus()
            {
                FullPath = (string)svnStatusType.GetProperty("FullPath").GetValue(o),
                LocalContentStatus = (SvnState)svnStatusType.GetProperty("LocalContentStatus").GetValue(o),
                Modified = (bool)svnStatusType.GetProperty("Modified").GetValue(o)
            });
        }

        public IEnumerable<SvnList> GetList(string path)
        {
            return GetList(GetSvnTarget(path));
        }

        public IEnumerable<SvnList> GetList(Uri uri, long revision = -1)
        {
            return GetList(GetSvnTarget(uri, revision));
        }

        protected virtual IEnumerable<SvnList> GetList(object target)
        {
            object listArgs = this.svnListArgsType.GetConstructor(Type.EmptyTypes).Invoke(null);

            this.svnListArgsType.GetProperty("RetrieveEntries").SetValue(listArgs, 63);
            
            var param = new object[] { target, listArgs, null };
            var method = svnClientType.GetMethod("GetList", new Type[] { svnTargetType, this.svnListArgsType, typeof(Collection<>).MakeGenericType(this.svnListType).MakeByRefType() });
            method.Invoke(svnClient, param);
            ICollection list = (ICollection)param[2];
            return list.Cast<object>().Select(o => new SvnList()
            {
                Path = (string)svnListType.GetProperty("Path").GetValue(o),
                NodeKind = (SvnNodeKind)this.svnDirEntryType.GetProperty("NodeKind").GetValue(this.svnListType.GetProperty("Entry").GetValue(o)),
                FileSize = Math.Max(0,  (long)this.svnDirEntryType.GetProperty("FileSize").GetValue(this.svnListType.GetProperty("Entry").GetValue(o))),
                Uri = (Uri)svnListType.GetProperty("Uri").GetValue(o)
            });
        }

        //private SvnList Make(object o)
        //{
        //    SvnList item = new SvnList();

        //    item.Path = (string)svnListType.GetProperty("Path").GetValue(o);
        //    item.NodeKind = (SvnNodeKind)this.svnDirEntryType.GetProperty("NodeKind").GetValue(this.svnListType.GetProperty("Entry").GetValue(o));
        //    item.FileSize = (long)this.svnDirEntryType.GetProperty("FileSize").GetValue(this.svnListType.GetProperty("Entry").GetValue(o));
        //    item.Uri = (Uri)svnListType.GetProperty("Uri").GetValue(o);
        //    return item;
        //}

        public bool Write(string path, Stream stream)
        {
            return Write(GetSvnTarget(path), stream);
        }

        public bool Write(Uri uri, Stream stream)
        {
            return Write(GetSvnTarget(uri), stream);
        }

        public bool Write(Uri uri, long revision, Stream stream)
        {
            return Write(GetSvnTarget(uri, revision), stream);
        }

        protected virtual bool Write(object target, Stream stream)
        {
            return (bool)svnClientType.GetMethod("Write", new Type[] { svnTargetType, typeof(Stream) }).Invoke(this.svnClient, new object[] { target, stream });
        }
                
        public virtual bool Update(string path)
        {
            return (bool)svnClientType.GetMethod("Update", new Type[] { typeof(string) }).Invoke(this.svnClient, new object[] { path });            
        }

        public virtual bool Commit(string path)
        {
            return (bool)svnClientType.GetMethod("Commit", new Type[] { typeof(string) }).Invoke(this.svnClient, new object[] { path });
        }

        public virtual bool Commit(string path, string comment)
        {
            object commit = this.svnCommitType.GetConstructor(Type.EmptyTypes).Invoke(null);
            this.svnCommitType.GetProperty("LogMessage").SetValue(commit, comment);
            return (bool)svnClientType.GetMethod("Commit", new Type[] { typeof(string), this.svnCommitType }).Invoke(this.svnClient, new object[] { path, commit });
        }
        
        protected object GetSvnTarget(string path)
        {
            return svnPathTargetType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { path });            
        }

        protected object GetSvnTarget(Uri uri, long revision = -1)
        {
            if (revision < 0)
            {
                return svnUriTargetType.GetConstructor(new Type[] { typeof(Uri) }).Invoke(new object[] { uri });
            }
            return svnUriTargetType.GetConstructor(new Type[] { typeof(Uri), typeof(long) }).Invoke(new object[] { uri, revision });
        }

        public bool TryGetRepositoryId(Uri uri, out Guid id)
        {
            object[] param = new object[] { uri, null };
            bool res = (bool)svnClientType.GetMethod("TryGetRepositoryId", new Type[] { typeof(Uri), typeof(Guid).MakeByRefType() }).Invoke(this.svnClient, param);
            id = res ? (Guid)param[1] : Guid.Empty;
            return res;
        }
    }
}
