using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpSvn
{
    internal class SvnMultiCommandClientBase : IDisposable
    {
        protected Assembly svnAssembly;
        protected Type svnMultiCommandClientType;
        protected Type svnAuthType;
        protected Type svnNodeKindType;
        protected Type svnCommittingEventArgsType;

        protected object svnMultiCommandClient;

        public SvnMultiCommandClientBase(Assembly assembly)
        {

            this.svnAssembly = assembly;
            this.svnMultiCommandClientType = this.svnAssembly.GetType("SharpSvn.SvnMultiCommandClient");
            this.svnAuthType = this.svnAssembly.GetType("SharpSvn.Security.SvnAuthentication");
            this.svnNodeKindType = this.svnAssembly.GetType("SharpSvn.SvnNodeKind");
            this.svnCommittingEventArgsType = this.svnAssembly.GetType("SharpSvn.SvnCommittingEventArgs");

            if (this.svnAssembly == null ||
                this.svnMultiCommandClientType == null ||
                this.svnAuthType == null ||
                this.svnNodeKindType == null ||
                this.svnCommittingEventArgsType == null)
            {
                Trace.TraceError("Initializing SvnMultiCommandClient failed!");
            }
            this.svnMultiCommandClient = Activator.CreateInstance(svnMultiCommandClientType);
        }

        public SvnMultiCommandClientBase(Assembly assembly, string login, string password) : this(assembly)
        {
            Authenticate(login, password);
        }

        public void Dispose()
        {
            if (this.svnMultiCommandClient != null)
            {
                this.svnMultiCommandClientType.GetMethod("Dispose").Invoke(this.svnMultiCommandClient, null);
                this.svnMultiCommandClient = null;
            }
        }

        public virtual void Authenticate(string login, string password)
        {
            object auth = svnMultiCommandClientType.GetProperty("Authentication").GetValue(this.svnMultiCommandClient);
            svnAuthType.GetMethod("ForceCredentials").Invoke(auth, new object[] { login, password });
        }

        public bool Open(Uri sessionUri)
        {
            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("Open", new Type[] { typeof(Uri) });
            return (bool)methodInfo.Invoke(this.svnMultiCommandClient, new object[] { sessionUri });
        }

        public bool FilesExists(string path)
        {
            var param = new object[] { path, null };
            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("GetNodeKind", new Type[] { typeof(string), this.svnNodeKindType.MakeByRefType() });
            bool res = (bool)methodInfo.Invoke(this.svnMultiCommandClient, param);
            return res && this.svnNodeKindType.GetEnumName(param[1]) == "File";
        }

        public bool CreateFile(string path, Stream stream)
        {
            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("CreateFile", new Type[] { typeof(string), typeof(Stream) });
            return (bool)methodInfo.Invoke(this.svnMultiCommandClient, new object[] { path, stream });

        }

        public bool UpdateFile(string path, Stream stream)
        {
            return (bool)this.svnMultiCommandClientType.GetMethod("UpdateFile", new Type[] { typeof(string), typeof(Stream) }).Invoke(this.svnMultiCommandClient, new object[] { path, stream });
        }

        public bool Delete(string path)
        {
            return (bool)this.svnMultiCommandClientType.GetMethod("Delete", new Type[] { typeof(string) }).Invoke(this.svnMultiCommandClient, new object[] { path });
        }

        public bool SetProperty(string path, string propertyName, string value)
        {
            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("SetProperty", new Type[] { typeof(string), typeof(string), typeof(string) });
            return (bool)methodInfo.Invoke(this.svnMultiCommandClient, new object[] { path, propertyName, value });
        }

        public bool Commit()
        {
            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("Commit", Type.EmptyTypes);
            return (bool)methodInfo.Invoke(this.svnMultiCommandClient, null);
        }

        private string logMessage;
        public bool Commit(string logMessage)
        {
            bool ret = false;
            this.logMessage = logMessage;

            MethodInfo method = this.GetType().GetMethod("OnCommitting");
            EventInfo eventInfo = this.svnMultiCommandClientType.GetEvent("Committing");
            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, method);
            eventInfo.AddEventHandler(this.svnMultiCommandClient, handler);


            MethodInfo methodInfo = this.svnMultiCommandClientType.GetMethod("Commit", Type.EmptyTypes);
            ret = (bool)methodInfo.Invoke(this.svnMultiCommandClient, null);

            eventInfo.RemoveEventHandler(this.svnMultiCommandClient, handler);

            return ret;
        }

        public void OnCommitting(object sender, object e)
        {
            this.svnCommittingEventArgsType.GetProperty("LogMessage").SetValue(e, this.logMessage);
        }
    }
}
