using System;
using System.Runtime.Serialization;

namespace KS.CRMWorkflow.Data.DataAccess.Exceptions
{
    public class ActionNotSupportedException : Exception
    {
        public ActionNotSupportedException() : 
            base() { }

        public ActionNotSupportedException(string message) : 
            base(message) { }

        public ActionNotSupportedException(string message, Exception innerException) :
            base(message, innerException) { }

        public ActionNotSupportedException(SerializationInfo info, StreamingContext context) : 
            base(info, context) { }
    }
}
