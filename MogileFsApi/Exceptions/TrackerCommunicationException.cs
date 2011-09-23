using System;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class TrackerCommunicationException : MogileFsBaseException
    {
        public TrackerCommunicationException()
        {
        }

        public TrackerCommunicationException(string message) : base(message)
        {
        }

        public TrackerCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}