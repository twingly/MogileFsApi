using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Primelabs.Twingly.MogileFsApi.Utils
{
    public static class ByteReader
    {
        /// <summary>
        /// Reads data into a complete array, throwing an EndOfStreamException
        /// if the stream runs out of data first, or if an IOException
        /// naturally occurs.
        /// Taken from: http://www.yoda.arachsys.com/csharp/readbinary.html
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        /// <param name="data">The array to read bytes into. The array
        /// will be completely filled from the stream, so an appropriate
        /// size must be given.</param>
        public static void ReadWholeArray(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (string.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }
    }
}
