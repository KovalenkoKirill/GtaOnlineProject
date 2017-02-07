using GtaLauncher.injector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GtaLauncher
{
    public class Game:IDisposable
    {
        public const string Inject_DLL_Name = "dsound.dll";

        Process gtaProcess;

        AppDomain GtaDomain;

        public int TimeOutSec = 60;

        public string PathToExe = @"C:\Games\Grand Theft Auto V\GTAVLauncher.exe";

        public string processName = @"GTA5";

        public string AssemblyPath = $@"{AppDomain.CurrentDomain.BaseDirectory}lib\";

        private DllInjectionResult loaderResult;

        public DllInjectionResult LoaderResult
        {
            get
            {
                return loaderResult;
            }
            set
            {
                loaderResult = value;
            }
        }

        public Game()
        {
            GtaDomain = AppDomain.CreateDomain("GTA");
        }

        public void Start()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(PathToExe);
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.EnvironmentVariables["LauncherPath"] = AssemblyPath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(PathToExe);
            injectDll();
            gtaProcess = Process.Start(startInfo);
            
        }

        private void injectDll()
        {
            string path = Path.GetDirectoryName(PathToExe);
            if (!File.Exists($@"{path}\{Inject_DLL_Name}"))
            {
                File.Copy($@"{this.AssemblyPath}{Inject_DLL_Name}", $@"{path}\{Inject_DLL_Name}");
            }
            else
            {
                if(GetHashFromFile($@"{path}\{Inject_DLL_Name}") != GetHashFromFile($@"{this.AssemblyPath}{Inject_DLL_Name}"))
                {
                    File.Copy($@"{this.AssemblyPath}{Inject_DLL_Name}", $@"{path}\{Inject_DLL_Name}",true);
                }
            }
        }

        private string GetHashFromFile(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

        public void Dispose()
        {
            gtaProcess.Kill();
        }
    }
}
