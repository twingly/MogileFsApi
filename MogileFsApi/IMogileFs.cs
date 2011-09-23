using System.Collections.Generic;
using System.IO;

namespace Primelabs.Twingly.MogileFsApi
{
    public interface IMogileFs
    {
        void StoreFile(string domain, string key, string storageClass, Stream inputStream, int timeoutMilliseconds);
        byte[] GetFileBytes(string domain, string key, int timeoutMilliseconds);

        void Delete(string domain, string key);
        void Rename(string domain, string fromKey, string toKey);
        void Sleep(int seconds);
        IList<string> GetPaths(string domain, string key, bool noVerify);
    }
}