using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SharpSvn
{
    internal class SvnClient17 : SvnClientBase
    {
        private static Assembly svn17Assembly;

        static SvnClient17()
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Environment.Is64BitProcess)
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.7\x64\SharpSvn.dll");
                }
                else
                {
                    appPath = Path.Combine(appPath, @"SharpSvn\v1.7\x86\SharpSvn.dll");
                }

                if (!File.Exists(appPath))
                {
                    throw new Exception($"File {appPath} not exists");
                }

                Trace.WriteLine($"Load assembly {appPath}");
                svn17Assembly = Assembly.LoadFrom(appPath);

                if (svn17Assembly == null)
                {
                    throw new Exception($"Failed to load {appPath}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }

        public SvnClient17() : base(svn17Assembly)
        { }

        public SvnClient17(string login, string password)
            : base(svn17Assembly, login, password)
        { }
    }
}
