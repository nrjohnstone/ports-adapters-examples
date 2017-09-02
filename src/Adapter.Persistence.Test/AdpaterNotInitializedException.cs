using System;
using System.Runtime.Serialization;

namespace Adapter.Persistence.Test
{
    internal class AdpaterNotInitializedException : Exception
    {
        public AdpaterNotInitializedException()
        {
        }

        public AdpaterNotInitializedException(string message) : base(message)
        {
        }

        public AdpaterNotInitializedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AdpaterNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}