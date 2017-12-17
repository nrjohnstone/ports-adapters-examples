using System;
using System.Runtime.Serialization;

namespace Domain.Entities
{
    public class AddBookRequestException : Exception
    {
        public AddBookRequestException()
        {
        }

        public AddBookRequestException(string message) : base(message)
        {
        }

        public AddBookRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AddBookRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}