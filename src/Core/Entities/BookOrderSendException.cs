using System;
using System.Runtime.Serialization;

namespace Core.Entities
{
    public class BookOrderSendException : Exception
    {
        public BookOrderSendException()
        {
        }

        public BookOrderSendException(string message) : base(message)
        {
        }

        public BookOrderSendException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BookOrderSendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}