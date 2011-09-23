using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Primelabs.Twingly.MogileFsApi.Exceptions
{
    public class MogileFsBaseException : Exception
    {
        public MogileFsBaseException()
        {
        }

        public MogileFsBaseException(string message) : base(message)
        {
        }

        public MogileFsBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
