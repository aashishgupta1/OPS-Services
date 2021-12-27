using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceCore
{
    public class ModuleManager
    {
        #region Properties

        /// <summary>
        /// Gets or sets the application domain list.
        /// </summary>
        /// <value>
        /// The application domain list.
        /// </value>
        public List<AppDomainWrapper<IModule>> AppDomainList { get; set; }

        /// <summary>
        /// The module path
        /// </summary>
        private readonly String ModulePath = ConfigurationManager.AppSettings["ModulePath"];

        /// <summary>
        /// Gets or sets a value indicating whether [stop requested].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [stop requested]; otherwise, <c>false</c>.
        /// </value>
        public bool StopRequested { get; set; }
        #endregion

        #region  "Singleton Class"
        /// <summary>
        /// The instance
        /// </summary>
        private static volatile ModuleManager instance;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private static readonly object syncRoot = new object();
        /// <summary>
        /// Prevents a default instance of the <see cref="ModuleManager"/> class from being created.
        /// </summary>
        private ModuleManager()
        {
            AppDomainList = new List<AppDomainWrapper<IModule>>();
        }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ModuleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ModuleManager();
                    }
                }

                return instance;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the module exeuction.
        /// </summary>
        public void StartModuleExeuction()
        {
            Task.Run(() =>
            {
                LoadModules();
            });
        }

        /// <summary>
        /// Stops the module exeuction.
        /// </summary>
        public void StopModuleExeuction()
        {
            StopRequested = true;
            List<Task> tasks = new List<Task>();
            foreach (AppDomainWrapper<IModule> appDomWrp in AppDomainList)
            {
                tasks.Add(Task.Run(() =>
                {
                    if (appDomWrp.WrappedModule.IsRunning())
                    {
                        appDomWrp.WrappedModule.StopProcess();
                        while (appDomWrp.WrappedModule.IsRunning()) ;
                    }
                    appDomWrp.Unload();
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public Settings ReadSettings(string settingFilePath)
        {
            Settings settings = null;
            Logger.Instance.LogInfo("Trying to load settings from {0}", settingFilePath);
            if (File.Exists(settingFilePath))
            {
                settings = SettingHandler.LoadSettings(settingFilePath);
            }
            else
            {
                Logger.Instance.LogError("Setting.xml not found path = {0}", settingFilePath);
            }
            return settings;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Load modules from the module path given in app config.
        /// </summary>
        private void LoadModules()
        {
            try
            {
                string modulePath = ModulePath;
                if (Directory.Exists(modulePath))
                {
                    string[] directories = Directory.GetDirectories(modulePath);
                    foreach (var directory in directories)
                    {
                        if (!StopRequested)
                        {
                            LoadModule(directory);
                        }
                    }
                }
            }

            catch (Exception e1)
            {
                Logger.Instance.LogError(e1.Message);
            }
        }

        /// <summary>
        /// Load module from given folder.
        /// </summary>
        /// <param name="directory">The directory.</param>
        private void LoadModule(string directory)
        {
            //System.Diagnostics.Debugger.Launch();

            ILogger logger = Logger.Instance;
            logger.LogInfo("Try to load module from {0}", directory);
            try
            {
                String settingFilePath = Path.Combine(directory, "Setting.xml");
#if DEBUG
                //// In debug mode us debug setting XML file.
                //if (File.Exists(Path.Combine(directory, "Setting_debug.xml")))
                //{
                //    settingFilePath = Path.Combine(directory, "Setting_debug.xml");
                //}
#endif
                if (File.Exists(settingFilePath))
                {
                    Settings settings = SettingHandler.LoadSettings(settingFilePath);
                    settings.AssemblyPath = Path.Combine(directory, settings.ModuleAssembly);

                    ILogger moduleLogger = new ModuleLogger(logger, new string[] {
                        Convert.ToString(settings.ModuleId),
                        settings.ModuleAssembly
                    });

                    string moduleDirectory = !string.IsNullOrEmpty(settings.ModuleStatePath) ? settings.ModuleStatePath : directory;

                    bool useAppDomain = false;
                    if (!StopRequested)
                    {
                        if (bool.TryParse(ConfigurationManager.AppSettings["UseAppdomain"], out useAppDomain))
                        {
                            if (useAppDomain)
                            {
                                AppDomainWrapper<IModule> domainWrapper = new AppDomainWrapper<IModule>(settings.AssemblyPath, settings.ModuleName, settings.ModuleId);
                                domainWrapper.OnInstanceCreated += ModuleAppDomain_InstanceCreated;
                                AppDomainList.Add(domainWrapper);
                                domainWrapper.CreateInstance(moduleLogger, settings);
                            }
                            else
                            {
                                CreateInstanceStartProcess(settings.AssemblyPath, moduleLogger, settings);
                            }
                        }
                    }
                }
                else
                {
                    logger.LogError("Setting.xml not found path = {0}", settingFilePath);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception);
            }
        }

        /// <summary>
        /// Handles the InstanceCreated event of the ModuleAppDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ModuleAppDomain_InstanceCreated(object sender, EventArgs args)
        {
            if (!StopRequested)
            {
                DomainEventArgs<IModule> domainArgs = (DomainEventArgs<IModule>)args;
                domainArgs.WrappedModule.StartProcess();
            }
        }

        /// <summary>
        /// Creates instance and start process
        /// </summary>
        /// <param name="args"></param>
        private void CreateInstanceStartProcess(string assemblyFullPath, params object[] args)
        {

            Assembly assmebly = Assembly.LoadFile(assemblyFullPath);
            Type classType = GetModuleTypeName(assmebly, typeof(IModule));
            IModule obj = (IModule)System.Activator.CreateInstance(classType, args);
            obj.StartProcess();
        }

        /// <summary>
        /// get module name from assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        Type GetModuleTypeName(Assembly assembly, Type baseType)
        {
            var types = assembly.GetTypes().Where(t => t != baseType &&
                                                 baseType.IsAssignableFrom(t));

            return types.First();
        }
        #endregion
    }
}
