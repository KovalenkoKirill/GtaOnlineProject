using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GtaLauncher.injector;
using System.Diagnostics;
using System.IO;

namespace TestGtaProject
{
    [TestClass]
    public class LoaderTest
    {
        [TestMethod]
        public void LoadScriotHook()
        {
            DllInjector injector = DllInjector.GetInstance;
            string CurrentName = Process.GetCurrentProcess().ProcessName;
            string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string dllPath = $@"{solutionDirectory}\lib\ScriptHookVDotNet3.dll";
            DllInjectionResult result =  injector.Inject(CurrentName, dllPath);
        }
    }
}
