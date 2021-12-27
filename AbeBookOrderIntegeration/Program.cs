using System;
using System.ServiceProcess;

namespace OPSService
{
    static class Program
    {
        public const string ServiceNamePrefix = "Order JobServer";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                OPSService service = new OPSService();
                service.Start();
                Console.ReadLine();
            }
            else if (System.Environment.UserInteractive)
            {
                //System.Diagnostics.Debugger.Launch();

                string serviceName = args[0];
                if (serviceName.StartsWith(":"))
                {
                    serviceName = ServiceNamePrefix + " " + serviceName;
                }
                string parameter = args[1];
                switch (parameter)
                {
                    case "-install":
                        SelfInstaller.Install(serviceName);
                        break;
                    case "-uninstall":
                        SelfInstaller.Uninstall(serviceName);
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new OPSService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
