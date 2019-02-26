using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpSvn
{
    internal class SvnClient19 : SvnClientBase
    {
        private static Assembly svn19Assembly;

        static SvnClient19()
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Environment.Is64BitProcess)
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.9\x64\SharpSvn.dll");
                }
                else
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.9\x86\SharpSvn.dll");
                }
                Trace.WriteLine($"Load assembly {appPath}");
                svn19Assembly = Assembly.LoadFrom(appPath);

                if (svn19Assembly == null)
                {
                    throw new Exception($"Failed to load {appPath}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public SvnClient19()
            : base(svn19Assembly)
        { }

        public SvnClient19(string login, string password)
            : base(svn19Assembly, login, password)
        { }
    }
}
