using System;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class NoPathsException : MogileFsBaseException
    {
        public NoPathsException()
        {
        }

        public NoPathsException(string message) : base(message)
        {
        }

        public NoPathsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}