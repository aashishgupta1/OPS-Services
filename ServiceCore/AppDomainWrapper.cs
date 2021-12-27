using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

namespace ServiceCore
{
    [Serializable]
    public class AppDomainWrapper<T> where T : IModule
    {
        #region Properties
        /// <summary>
        /// The logger
        /// </summary>
        private readonly static ILogger logger = Logger.Instance;

        /// <summary>
        /// Gets or sets the application domain.
        /// </summary>
        /// <value>
        /// The application domain.
        /// </value>
        public AppDomain AppDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoaded { get; set; }

        /// <summary>
        /// The wrapped object
        /// </summary>
        T _wrappedObject;

        /// <summary>
        /// The assembly full path
        /// </summary>
        readonly string _assemblyFullPath;

        /// <summary>
        /// Gets the loaded module.
        /// </summary>
        /// <value>
        /// The loaded module.
        /// </value>
        public T WrappedModule { get { return _wrappedObject; } }

        /// <summary>
        /// Occurs when [on load].
        /// </summary>
        public event EventHandler OnInstanceCreated;

        /// <summary>
        /// Occurs when [on unload].
        /// </summary>
        public event EventHandler OnUnload;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainWrapper{T}" /> class.
        /// </summary>
        /// <param name="assemblyFullPath">The assembly full path.</param>
        /// <param name="appDomainName">Name of the application domain.</param>
        public AppDomainWrapper(string assemblyFullPath, string moduleName, int moduleId)
        {
            _assemblyFullPath = assemblyFullPath;
            string appDomainName = string.Format("OneDMSJob_{0}_{1}_{2}", moduleName, moduleId, Guid.NewGuid().ToString());

            AppDomainSetup domaininfo = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(assemblyFullPath),
                ApplicationName = Path.GetFileName(assemblyFullPath)
            };
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;

            logger.LogInfo("App Domain \"{0}\" Creating for assembly \"{1}\"", appDomainName, _assemblyFullPath);

            AppDomain = AppDomain.CreateDomain(appDomainName);

            AppDomain.AssemblyResolve += new ResolveEventHandler(AppDomain_AssemblyResolve);
            AppDomain.DomainUnload += AppDomain_DomainUnload;
            AppDomain.UnhandledException += AppDomain_UnhandledException;

            logger.LogInfo("App Domain \"{0}\" Created for assembly \"{1}\"", appDomainName, _assemblyFullPath);
        }

        #endregion

        #region Events
        /// <summary>
        /// Handles the AssemblyResolve event of the AppDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ResolveEventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (File.Exists(args.Name))
            {
                Assembly result = Assembly.LoadFrom(args.Name);
                AssemblyName[] arrReferencedAssmbNames = result.GetReferencedAssemblies();
                return result;
            }
            else
                return null;

        }

        /// <summary>
        /// Handles the DomainUnload event of the AppDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AppDomain_DomainUnload(object sender, EventArgs e)
        {
            logger.LogInfo("App Domain Unloaded for assembly {0}", _assemblyFullPath);
            IsLoaded = false;
        }

        /// <summary>
        /// Handles the UnhandledException event of the AppDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.LogError((Exception)e.ExceptionObject, "Error in app domain name {0}", AppDomain.FriendlyName);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void CreateInstance(params object[] args)
        {

            if (!IsLoaded)
            {
                try
                {
                    logger.LogInfo("Loading assembly {0} ", _assemblyFullPath);

                    Assembly assmebly = Assembly.LoadFile(_assemblyFullPath);
                    Type classType = GetModuleTypeName(assmebly, typeof(T));

                    _wrappedObject = (T)AppDomain.CreateInstanceAndUnwrap(@_assemblyFullPath, classType.FullName, false, BindingFlags.Default,
                        null, args,
                        null, null);

                    IsLoaded = true;

                    logger.LogInfo("Loaded assembly {0}", _assemblyFullPath);

                    try
                    {
                        OnInstanceCreated?.Invoke(this, new DomainEventArgs<T>() { WrappedModule = _wrappedObject });
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error in starting  - app domain name \"{0}\".", AppDomain.FriendlyName);
                        Unload();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in loading assembly \"{0}\".", _assemblyFullPath);
                    Unload();
                }
            }
        }

        /// <summary>
        /// Unloads this instance.
        /// </summary>
        public void Unload()
        {
            if (IsLoaded)
            {
                logger.LogInfo("App Domain Unloading for assembly {0}", _assemblyFullPath);

                OnUnload?.Invoke(this, new DomainEventArgs<T>() { WrappedModule = _wrappedObject });

                AppDomain.Unload(AppDomain);
            }
        }

        /// <summary>
        /// Gets the name of the module type.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="baseType">Type of the base.</param>
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
