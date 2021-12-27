using System;

namespace OPSService.Infrastructure.Loggging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void LogError(Exception exception, string message, params object[] args);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void LogError(string message, params object[] args);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void LogError(Exception exception);

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void LogInfo(string message, params object[] args);

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogWarn(string message);

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void LogDebug(string message, params object[] args);

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        void LogTrace(string message, params object[] args);
    }
}
