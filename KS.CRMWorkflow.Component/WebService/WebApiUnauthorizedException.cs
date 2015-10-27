using System;
using System.Runtime.Serialization;

namespace KS.CRMWorkflow.Component.WebService
{
    public class WebApiUnauthorizedException : Exception
    {
        public WebApiUnauthorizedException() 
            : base()
        { }

        public WebApiUnauthorizedException(string message) 
            : base(message)
        { }

        public WebApiUnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public WebApiUnauthorizedException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        { }
    }
}
