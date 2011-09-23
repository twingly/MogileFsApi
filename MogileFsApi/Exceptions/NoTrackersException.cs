using System;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class NoTrackersException : MogileFsBaseException
    {
        public NoTrackersException()
        {
        }

        public NoTrackersException(string message) : base(message)
        {
        }

        public NoTrackersException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}