using System;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class MogileFsProtocolException : MogileFsBaseException
    {
        public MogileFsProtocolException()
        {
        }

        public MogileFsProtocolException(string message) : base(message)
        {
        }

        public MogileFsProtocolException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}