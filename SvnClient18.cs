using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpSvn
{
    internal class SvnClient18 : SvnClientBase
    {
        private static Assembly svn18Assembly;

        static SvnClient18()
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Environment.Is64BitProcess)
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.8\x64\SharpSvn.dll");
                }
                else
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.8\x86\SharpSvn.dll");
                }
                Trace.WriteLine($"Load assembly {appPath}");
                svn18Assembly = Assembly.LoadFrom(appPath);

                if (svn18Assembly == null)
                {
                    throw new Exception($"Failed to load {appPath}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public SvnClient18() : base(svn18Assembly)
        { }

        public SvnClient18(string login, string password)
            : base(svn18Assembly, login, password)
        { }
    }
}
