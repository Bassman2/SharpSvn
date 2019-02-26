using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpSvn
{
    internal class SvnMultiCommandClient18 : SvnMultiCommandClientBase
    {
        private static Assembly svn18Assembly;

        static SvnMultiCommandClient18()
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Environment.Is64BitProcess)
                {
                    svn18Assembly = Assembly.LoadFrom(Path.Combine(appPath, @"SharpSvn\v1.8\x64\SharpSvn.dll"));
                }
                else
                {
                    svn18Assembly = Assembly.LoadFrom(Path.Combine(appPath, @"SharpSvn\v1.8\x86\SharpSvn.dll"));
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public SvnMultiCommandClient18()
            : base(svn18Assembly)
        { }

        public SvnMultiCommandClient18(string login, string password)
            : base(svn18Assembly, login, password)
        { }
    }
}
