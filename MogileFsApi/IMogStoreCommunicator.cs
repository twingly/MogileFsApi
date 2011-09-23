using System;
using System.IO;

namespace Primelabs.Twingly.MogileFsApi
{
    public interface IMogStoreCommunicator
    {
        byte[] DownloadFile(string path, int timeoutMilliseconds);
        void UploadFile(Uri uri, Stream data, int timeoutMilliseconds);
    }
}