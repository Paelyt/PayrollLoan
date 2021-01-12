using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topshelf;

namespace UvlotApplication.Service
{
    public class WinProgram
    {
        //var exitCode = HostFactory.Run(x =>
        //{
        //    x.Service<WinServe>(s =>
        //    {
        //        s.ConstructUsing(winserve => new WinServe());
        //        s.WhenStarted(WinServe => WinServe.Start());
        //        s.WhenStopped(WinServe => WinServe.Stop());
        //    });

        //    x.RunAsLocalSystem();

        //    x.SetServiceName("HeartbeatService");
        //    x.SetDisplayName("Heartbeat Service");
        //    x.SetDescription("This is the sample heartbeat service used in a YouTube demo.");
        //});

        //int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
        //Environment.ExitCode = exitCodeValue;
        //}
}
}