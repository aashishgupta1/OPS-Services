using System;

namespace OPSService.Infrastructure.Loggging
{
    [Serializable]
    public class ModuleLogger : ILogger
    {
        /// <summary>
        /// The logger
        /// </summary>
        ILogger logger;

        /// <summary>
        /// The initialize arguments
        /// </summary>
        readonly string[] initArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleLogger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="initArgs">The initialize arguments.</param>
        public ModuleLogger(ILogger logger, params string[] initArgs)
        {
            this.logger = logger;
            this.initArgs = initArgs;
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogError(exception, message, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(string message, params object[] args)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogError(message, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void LogError(Exception exception)
        {
            string message = string.Format("{0}|", string.Join("|", initArgs));
            logger.LogError(exception, message);
        }

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void LogInfo(string message, params object[] args)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogInfo(message, args);
        }

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogWarn(string message)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogWarn(message);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogDebug(string message, params object[] args)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogDebug(message, args);
        }

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogTrace(string message, params object[] args)
        {
            message = string.Format("{0}|{1}", string.Join("|", initArgs), message);
            logger.LogTrace(message, args);
        }
    }
}
