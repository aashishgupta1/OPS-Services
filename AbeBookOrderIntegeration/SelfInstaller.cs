using OPSService.Infrastructure.Loggging;
using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace OPSService
{
    public static class SelfInstaller
    {
        public static void Install(string serviceName)
        {
            try
            {
                CreateInstaller(serviceName).Install(new Hashtable());
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
                throw;
            }
        }

        public static void Uninstall(string serviceName)
        {
            try
            {
                CreateInstaller(serviceName).Uninstall(null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        private static Installer CreateInstaller(string serviceName)
        {
            var installer = new TransactedInstaller();
            installer.Installers.Add(new ServiceInstaller
            {
                ServiceName = serviceName,
                DisplayName = serviceName,
                Description = Assembly.GetExecutingAssembly().GetName().Name,
                StartType = ServiceStartMode.Automatic
            });
            installer.Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            });
            var installContext = new InstallContext(serviceName + ".install.log", null);
            installContext.Parameters["assemblypath"] = Assembly.GetEntryAssembly().Location;
            installContext.Parameters["serviceName"] = serviceName;
            installer.Context = installContext;
            installer.AfterInstall += Installer_AfterInstall;
            installer.BeforeUninstall += Installer_BeforeUninstall;
            return installer;
        }

        private static void Installer_BeforeUninstall(object sender, InstallEventArgs e)
        {
            if ((sender as Installer) != null)
            {
                string name = (sender as Installer).Context.Parameters["serviceName"].ToString();
                ServiceController sc = new ServiceController(name);
                if (sc.Status == ServiceControllerStatus.Running) { sc.Stop(); }
            }
        }

        private static void Installer_AfterInstall(object sender, InstallEventArgs e)
        {
            if ((sender as Installer) != null)
            {
                string name = (sender as Installer).Context.Parameters["serviceName"].ToString();
                ServiceController sc = new ServiceController(name);
                sc.Start();
            }
        }
    }
}
