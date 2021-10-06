using System;
using System.Runtime.Serialization;

namespace Domain.Entities
{
    public class BookOrderApproveException : Exception
    {
        public BookOrderApproveException()
        {
        }

        public BookOrderApproveException(string message) : base(message)
        {
        }

        public BookOrderApproveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BookOrderApproveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}