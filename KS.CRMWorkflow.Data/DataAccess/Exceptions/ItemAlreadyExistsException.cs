using System;
using System.Runtime.Serialization;

namespace KS.CRMWorkflow.Data.DataAccess.Exceptions
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException() : 
            base() { }

        public ItemAlreadyExistsException(string message ) : 
            base(message) { }

        public ItemAlreadyExistsException(string message, Exception innerException) :
            base(message, innerException) { }

        public ItemAlreadyExistsException(SerializationInfo info, StreamingContext context) : 
            base(info, context) { }
    }
}
