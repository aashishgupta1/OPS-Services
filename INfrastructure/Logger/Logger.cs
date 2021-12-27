using NLog;
using System;
using System.Reflection;

namespace OPSService.Infrastructure.Loggging
{
    [Serializable]
    public class Logger : ILogger
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static volatile Logger _instance;

        /// <summary>
        /// The _sync object.
        /// </summary>
        private static readonly object _syncObject = new object();

        /// <summary>
        /// The _log.
        /// </summary>
        private static readonly NLog.Logger _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        /// <summary>
        /// The _is debug enabled.
        /// </summary>
        private readonly bool _isDebugEnabled;

        /// <summary>
        /// The _is info enabled.
        /// </summary>
        private readonly bool _isInfoEnabled;

        /// <summary>
        /// The _is warn enabled.
        /// </summary>
        private readonly bool _isWarnEnabled;

        /// <summary>
        /// The _is error enabled.
        /// </summary>
        private readonly bool _isErrorEnabled;

        /// <summary>
        /// The is trace enable
        /// </summary>
        private readonly bool _isTraceEnable;

        #region Ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="Logger"/> class from being created. 
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        private Logger()
        {
            _isDebugEnabled = _log.IsDebugEnabled;
            _isInfoEnabled = _log.IsInfoEnabled;
            _isWarnEnabled = _log.IsWarnEnabled;
            _isErrorEnabled = _log.IsErrorEnabled;
            _isTraceEnable = _log.IsTraceEnabled;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Logger Instance
        {
            get
            {
                // Check for null before acquiring the lock.
                if (_instance == null)
                {
                    // Use a _syncObject to lock on, to avoid deadlocks among multiple threads.
                    lock (_syncObject)
                    {
                        // Again check if _instance has been initialized, 
                        // since some other thread may have acquired the lock first and constructed the object.
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region ILogger Members

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            if (_isErrorEnabled)
            {
                string innerExcetionmessage = exception.InnerException == null ? string.Empty : exception.InnerException.ToString();
                _log.Error("{0} Error: {1} {2} {3} {4} {5}",
                    string.Format(message, args),
                    exception.Message,
                    Environment.NewLine,
                    innerExcetionmessage,
                     Environment.NewLine,
                     exception.StackTrace);
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(string message, params object[] args)
        {
            if (_isErrorEnabled)
                _log.Error(message, args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void LogError(Exception exception)
        {
            if (_isErrorEnabled)
                _log.Error(exception);
        }

        /// <summary>
        /// Logs the info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void LogInfo(string message, params object[] args)
        {
            if (_isInfoEnabled)
                _log.Info(message, args);
        }

        /// <summary>
        /// Logs the warn.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogWarn(string message)
        {
            if (_isWarnEnabled)
                _log.Warn(message);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogDebug(string message, params object[] args)
        {
            if (_isDebugEnabled)
                _log.Debug(message, args);
        }

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public void LogTrace(string message, params object[] args)
        {
            if (_isTraceEnable)
                _log.Trace(message, args);
        }
        #endregion
    }
}
