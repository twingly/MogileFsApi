using System;
using System.IO;
using System.Net;
using Primelabs.Twingly.MogileFsApi.Exceptions;
using Primelabs.Twingly.MogileFsApi.Utils;

namespace Primelabs.Twingly.MogileFsApi
{
    public class MogStoreCommunicator : IMogStoreCommunicator
    {
        public byte[] DownloadFile(string path, int timeoutMilliseconds)
        {
            var request = (HttpWebRequest) WebRequest.Create(path);
            request.Timeout = timeoutMilliseconds;
            using (var response = request.GetResponse() as HttpWebResponse) {
                var stream = response.GetResponseStream();
                stream.ReadTimeout = timeoutMilliseconds;

                var retval = new byte[response.ContentLength];

                ByteReader.ReadWholeArray(stream, retval);

                return retval;
            }
        }

        public void UploadFile(Uri uri, Stream input, int timeoutMilliseconds)
        {
            input.Position = 0;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = timeoutMilliseconds;
            request.Method = "PUT";
            request.ContentLength = input.Length;

            // Start writing the output
            using (var outputStream = request.GetRequestStream()) 
            {
                int offset = 0;
                long remaining = input.Length;
                byte[] data = new byte[4096];
                while (remaining > 0)
                {
                    int read;
                    try {
                        read = input.Read(data, 0, 4096);
                    } catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);
                        throw new IOException("Error reading from stream for path: " + uri, ex);
                    }
                    if (read <= 0)
                        throw new EndOfStreamException
                            (string.Format("End of stream reached with {0} bytes left to read", remaining));
                    try {
                        outputStream.Write(data, 0, read);
                    } catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex);
                        throw new IOException("Error writing to stream for path: " + uri, ex);
                    }
                    remaining -= read;
                    offset += read;
                }
                outputStream.Flush();
            }
            
            // Request is now sent. Read the answer
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                switch (response.StatusCode) {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                        return;
                    default:
                        break;
                }

                var responseStream = response.GetResponseStream();
                responseStream.ReadTimeout = timeoutMilliseconds;
                using (var sr = new StreamReader(responseStream)) {
                    var str = sr.ReadToEnd();

                    throw new StorageCommunicationException(
                        string.Format("Error storing file to path {0}.\r\nFull error:\r\n{1}",
                                      uri, str));
                }
            }
        }
    }
}