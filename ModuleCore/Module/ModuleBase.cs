using OPSService.Infrastructure;
using OPSService.Infrastructure.Loggging;
using OPSService.ModuleCore.Interfaces;
using DBInteraction;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModuleCore
{
    public abstract class ModuleBase : MarshalByRefObject, IModule
    {
        #region Inner IJob Class
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="Quartz.IJob" />
        class ModuleJob : IJob
        {
            /// <summary>
            /// The job context data key
            /// </summary>
            public static string jobContextDataKey = "ModuleRef";

            /// <summary>
            /// Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
            /// fires that is associated with the <see cref="T:Quartz.IJob" />.
            /// </summary>
            /// <param name="context">The execution context.</param>
            /// <remarks>
            /// The implementation may wish to set a  result object on the
            /// JobExecutionContext before this method exits.  The result itself
            /// is meaningless to Quartz, but may be informative to
            /// <see cref="T:Quartz.IJobListener" />s or
            /// <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
            /// execution.
            /// </remarks>
            public void Execute(IJobExecutionContext context)
            {
                ModuleBase obj = (ModuleBase)context.Scheduler.Context.Get(jobContextDataKey);
                try
                {
                    obj.isJobCompleted = false;
                    obj.logger.LogInfo("Job execution started");

                    JobScheduler.PauseJob(context.JobDetail.Key);
                    obj.logger.LogInfo("Job execution paused");

                    obj.ExecuteRun();
                }
                catch (Exception ex)
                {
                    obj.logger.LogError(ex);
                    throw;
                }
                finally
                {
                    obj.logger.LogInfo("Job execution completed");
                    obj.isJobCompleted = true;

                    obj.logger.LogInfo("Job execution Resumed");
                    JobScheduler.ResumeJob(context.JobDetail.Key);
                }
            }
        }
        #endregion

        #region Instance Variables
        /// <summary>
        /// The job key
        /// </summary>
        private JobKey jobKey;

        /// <summary>
        /// The is cancel requested
        /// </summary>
        private bool isCancelRequested;

        /// <summary>
        /// The is job completed
        /// </summary>
        protected bool isJobCompleted = true;

        /// <summary>
        /// The current task
        /// </summary>
        protected Task currentTask;

        /// <summary>
        /// The logger
        /// </summary>
        protected ILogger logger;


        /// <summary>
        /// Gets or sets the module setings.
        /// </summary>
        /// <value>
        /// The module setings.
        /// </value>
        public ISettings ModuleSetings { get; private set; }

        public const string SUCCESS = "SUCCESS";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase"/> class.
        /// </summary>
        /// <param name="scaviewManager">The scaview manager.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="moduleState">State of the module.</param>
        /// <param name="settings">The settings.</param>
        public ModuleBase(ILogger logger, ISettings settings)
        {
            this.logger = logger;

            this.ModuleSetings = settings;

            logger.LogInfo("Module instance created");
        }
        #endregion

        #region Public Methods

        /// <summary>
        ///  Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Starts the process.
        /// </summary>
        /// <exception cref="System.Exception">Already Running</exception>
        public void StartProcess()
        {
            if (IsRunning())
            {
                logger.LogInfo("Module already running");
                throw new Exception("Module already running");
            }

            if (!string.IsNullOrEmpty(ModuleSetings.CronExpression))
            {
                jobKey = JobScheduler.StartWithData<ModuleJob>(new List<KeyValuePair<string, object>>() {
                     new KeyValuePair<string, object>(ModuleBase.ModuleJob.jobContextDataKey, this)
                }, ModuleSetings.CronExpression, ModuleSetings.StartImmediately);
            }
            else
            {
                currentTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        logger.LogInfo("Task Started");

                        ExecuteRun();

                        logger.LogInfo("Task Completed");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex);
                        throw;
                    }
                });
            }
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            logger.LogInfo("Process Stop Requested");
            try
            {
                isCancelRequested = true;
                if (!string.IsNullOrEmpty(ModuleSetings.CronExpression))
                {
                    JobScheduler.Stop(jobKey);
                }
                CompletePendingTask();
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Determines whether this instance is running.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRunning()
        {
            if (!string.IsNullOrEmpty(ModuleSetings.CronExpression))
            {
                return JobScheduler.IsRunning(jobKey);
            }
            else
            {
                return currentTask != null && currentTask.Status == TaskStatus.Running;
            }
        }

        /// <summary>
        /// Determines whether [is cancellation requested].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is cancellation requested]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCancellationRequested()
        {
            return isCancelRequested;
        }

        /// <summary>
        /// Completes the pending task.
        /// </summary>
        public void CompletePendingTask()
        {
            try
            {
                logger.LogInfo("CompletePendingTask Method Started");

                if (!string.IsNullOrEmpty(ModuleSetings.CronExpression))
                {
                    if (!isJobCompleted)
                    {
                        logger.LogInfo("Job is not compeleted yet.");
                        while (!isJobCompleted)
                        {
                            logger.LogInfo("Waiting for job to complete ------");

                            Task.Delay(500).Wait();
                        }
                        logger.LogInfo("Job compeleted");
                    }
                    else
                    {
                        logger.LogInfo("Job already compeleted");
                    }
                }
                else
                {
                    if (currentTask != null && !currentTask.IsCompleted)
                    {
                        logger.LogInfo("Waiting for task to compeleted");

                        currentTask.Wait();
                        currentTask.Dispose();

                        logger.LogInfo("Task compeleted");
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }

        }

        /// <summary>
        /// Executes run method.
        /// </summary>
        private void ExecuteRun()
        {
            try
            {
                Run();
            }

            finally
            {
            }
        }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public abstract void Run();
        #endregion

        #region File Helper Methods

        public void MoveAndReplaceFile(string sourceFilePath, string destinationFilePath)
        {
            FileManager.MoveAndReplaceFile(sourceFilePath, destinationFilePath);
        }

        public void MoveFileInErrorFolder(string fileNameOrFilePath, bool isFullFilePath = true)
        {
            if (isFullFilePath)
            {
                FileManager.MoveAndReplaceFile(fileNameOrFilePath, Path.Combine(ModuleSetings.ErrorFolderPath, Path.GetFileName(fileNameOrFilePath)));
            }
            else
            {
                FileManager.MoveAndReplaceFile(Path.Combine(ModuleSetings.DirectoryPath, fileNameOrFilePath), Path.Combine(ModuleSetings.ErrorFolderPath, fileNameOrFilePath));
            }
        }

        public void MoveFileInErrorFolder2(string fileNameOrFilePath, bool isFullFilePath = true)
        {
            if (isFullFilePath)
            {
                FileManager.MoveAndReplaceFile(fileNameOrFilePath, Path.Combine(ModuleSetings.ErrorFolderPath2, Path.GetFileName(fileNameOrFilePath)));
            }
            else
            {
                FileManager.MoveAndReplaceFile(Path.Combine(ModuleSetings.DirectoryPath, fileNameOrFilePath), Path.Combine(ModuleSetings.ErrorFolderPath2, fileNameOrFilePath));
            }
        }

        /// <summary>
        /// Moving File in Error Folder
        /// </summary>
        /// <param name="files"></param>
        public void MoveFilesOrLineInErrorFolder(string line, string file)
        {
            logger.LogInfo("Moving files in error folder");
            try
            {
                string errorfilePath = Path.Combine(ModuleSetings.ErrorFolderPath, Path.GetFileName(file));
                if (string.IsNullOrEmpty(line))
                {
                    File.Move(file, errorfilePath);
                    return;
                }
                if (!File.Exists(errorfilePath))
                {
                    var errorFile = File.Create(Path.Combine(ModuleSetings.ErrorFolderPath, Path.GetFileName(errorfilePath)));
                    errorFile.Close();
                }
                logger.LogInfo("Writing in error file.");
                File.AppendAllText(errorfilePath, line, ModuleSetings.DefaultEncoding);
                File.AppendAllText(errorfilePath, string.Join("", ModuleSetings.NewLineChars));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MoveFilesInErrorFolder method.");
            }
        }

        public void MoveFileInReportOutputPath(string fileNameOrFilePath, bool isFullFilePath = true)
        {
            if (isFullFilePath)
            {
                FileManager.MoveAndReplaceFile(fileNameOrFilePath, Path.Combine(ModuleSetings.ReportOutputPath, Path.GetFileName(fileNameOrFilePath)));
            }
            else
            {
                FileManager.MoveAndReplaceFile(Path.Combine(ModuleSetings.DirectoryPath, fileNameOrFilePath), Path.Combine(ModuleSetings.ReportOutputPath, fileNameOrFilePath));
            }
        }


        public void MoveFileInOutputFolder(string fileNameOrFilePath, bool isFullFilePath = true)
        {
            if (isFullFilePath)
            {
                if (ModuleSetings.IsDelete)
                {
                    FileManager.DeleteFile(fileNameOrFilePath);
                }
                else
                {
                    FileManager.MoveAndReplaceFile(fileNameOrFilePath, Path.Combine(ModuleSetings.DumpFolderPath, Path.GetFileName(fileNameOrFilePath)));
                }
            }
            else
            {
                string inputFilePath = Path.Combine(ModuleSetings.DirectoryPath, fileNameOrFilePath);
                if (ModuleSetings.IsDelete)
                {
                    FileManager.DeleteFile(inputFilePath);
                }
                else
                {
                    FileManager.MoveAndReplaceFile(inputFilePath, Path.Combine(ModuleSetings.DumpFolderPath, fileNameOrFilePath));
                }
            }

        }

        public void DeleteFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                FileManager.DeleteFile(filePath);
            }
        }
        public T Deserialize<T>(string input) where T : class
        {
            //XmlRootAttribute xRoot = new XmlRootAttribute();
            //xRoot.ElementName = "orderUpdateResponse";
            //xRoot.IsNullable = true;

            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }


        #endregion
        public DataTable GetMarketPlacedetail(string marketPlaceName)
        {
            DataTable dt = new DataTable();
            string sRetVal = DBOperation.ExecuteDBOperation("Select * from MarketPlaceMaster where IsActive = 1 and WebSiteLink='" + marketPlaceName + "'", DBOperation.OperationType.SELECT, null, ref dt);
            if(sRetVal != "SUCCESS")
            {
                logger.LogError(sRetVal);
            }
            return dt;
        }
    }
}
