using System;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class StorageCommunicationException : MogileFsBaseException
    {
        public StorageCommunicationException()
        {
        }

        public StorageCommunicationException(string message) : base(message)
        {
        }

        public StorageCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}