using AssemblyLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GtaLauncher
{
    public class Game:IDisposable
    {
        Process gtaProcess;

        AppDomain GtaDomain;

        public string PathToExe = @"C:\Games\Grand Theft Auto V\GTAVLauncher.exe";

        public string AssemblyPath = $@"{AppDomain.CurrentDomain.BaseDirectory}libs\";

        public Game()
        {
            GtaDomain = AppDomain.CreateDomain("GTA");
        }

        public void Start()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(PathToExe);
            startInfo.CreateNoWindow = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(PathToExe);
            startInfo.Arguments = "-scOfflineOnly";
            startInfo.Domain = "GTA";
            gtaProcess = Process.Start(startInfo);
            LoadAssembly();
        }

        public void LoadAssembly()
        {
            Type typeName = typeof(Loader);
            Loader loader = (Loader)GtaDomain.CreateInstanceFromAndUnwrap($"{AppDomain.CurrentDomain.BaseDirectory}AssemblyLoader.dll", typeName.FullName);
            loader.ShowConsole();
            loader.LoadNative($@"{AssemblyPath}\ScriptHookV.dll");
            loader.LoadNative($@"{AssemblyPath}\ScriptHookVDotNet.dll");
            
        }

        public void Dispose()
        {
            gtaProcess.Kill();
        }
    }
}
