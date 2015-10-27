using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using KS.CRMWorkflow.Component.Logging.Interface;
using System;
using System.Diagnostics;

namespace KS.CRMWorkflow.Component.Logging.Implemetation
{
    /// <summary>
    /// Implementation of a logging class using Microsoft Enterprise logging.
    /// </summary>
    public class EnterpriseLogger : ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        public static TraceEventType ApplicationTraceLevel = TraceEventType.Information;

        private static volatile EnterpriseLogger instance;
        private static object syncRoot = new Object();

        public static EnterpriseLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new EnterpriseLogger();
                    }
                }

                return instance;
            }
        }

        private EnterpriseLogger()
        {
            LoggingConfiguration configuration = new LoggingConfiguration();
            LogWriter writer = new LogWriter(configuration);
            Logger.SetLogWriter(writer, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(object source, object message, Exception exception = null)
        {
            Write(source, message, exception, TraceEventType.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Crictical(object source, object message, Exception exception = null)
        {
            Write(source, message, exception, TraceEventType.Critical);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Information(object source, object message)
        {
            Write(source, message, "", TraceEventType.Information);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Warning(object source, object message)
        {
            Write(source, message, "", TraceEventType.Warning);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Verbose(object source, object message)
        {
            Write(source, message, "", TraceEventType.Verbose);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="details"></param>
        /// <param name="severity"></param>
        private void Write(object source, object message, object details, TraceEventType severity)
        {
            if ((int)severity <= (int)ApplicationTraceLevel)
            {
                LogEntry log = new LogEntry
                {
                    TimeStamp = DateTime.UtcNow,
                    Title = string.Format("{0} -> {1}", source.ToString(), message.ToString()),
                    Severity = severity,
                    Message = details.ToString(),
                    MachineName = System.Environment.MachineName,
                    ProcessId = Process.GetCurrentProcess().Id.ToString(),
                };

                Logger.Write(log);
            }
        }
    }
}
