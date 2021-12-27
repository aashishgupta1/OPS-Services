using OPSService.Infrastructure.Loggging;
using ServiceCore;
using System;
using System.ServiceProcess;
namespace OPSService
{
    public partial class OPSService : ServiceBase
    {
        public OPSService()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.Instance.LogInfo("Sevice Started");
                base.OnStart(args);
                ModuleManager.Instance.StartModuleExeuction();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Logger.Instance.LogInfo("Sevice Stopped");
                ModuleManager.Instance.StopModuleExeuction();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex.Message);
            }
            base.OnStop();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogError((Exception)e.ExceptionObject);
        }
        public void Start()
        {
            OnStart(null);
        }
    }
}
