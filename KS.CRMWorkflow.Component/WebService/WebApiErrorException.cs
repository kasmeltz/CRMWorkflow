using System;
using System.Runtime.Serialization;

namespace KS.CRMWorkflow.Component.WebService
{
    public class WebApiErrorException : Exception
    {
        public WebApiErrorException() 
            : base()
        { }

        public WebApiErrorException(string message) 
            : base(message)
        { }

        public WebApiErrorException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public WebApiErrorException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        { }
    }
}
