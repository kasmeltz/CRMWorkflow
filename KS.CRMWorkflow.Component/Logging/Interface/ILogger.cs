using System;

namespace KS.CRMWorkflow.Component.Logging.Interface
{
    /// <summary>
    /// Represents an item that can log activity during the execution of an application.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(object source, object message, Exception exception = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Crictical(object source, object message, Exception exception = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        void Information(object source, object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        void Warning(object source, object message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        void Verbose(object source, object message);
    }
}
