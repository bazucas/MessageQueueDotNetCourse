using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.EndToEnd.Tests
{
    [TestClass]
    [DeploymentItem("StartHandlers.cmd")]
    [DeploymentItem("StopHandlers.cmd")]
    public sealed class HandlerSetup
    {
        [AssemblyInitialize]
        public static void Start(TestContext context)
        {
            Process.Start(new ProcessStartInfo("StartHandlers.cmd") { WindowStyle = ProcessWindowStyle.Hidden });
            Thread.Sleep(5000);
        }

        [AssemblyCleanup]
        public static void Stop()
        {
            Process.Start(new ProcessStartInfo("StopHandlers.cmd") {  WindowStyle=ProcessWindowStyle.Hidden });
        }
    }
}
