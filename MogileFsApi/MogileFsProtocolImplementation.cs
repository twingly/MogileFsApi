using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Primelabs.Twingly.MogileFsApi.Exceptions;
using Primelabs.Twingly.MogileFsApi.ObjectPool;
using Primelabs.Twingly.MogileFsApi.Utils;
using Math=System.Math;
using Random=System.Random;

namespace Primelabs.Twingly.MogileFsApi
{
    public class MogileFsProtocolImplementation : IMogileFs
    {
        private readonly ITrackerBackend _trackerBackend;
        private readonly IMogStoreCommunicator _storeCommunicator;

        public MogileFsProtocolImplementation(ITrackerBackend trackerBackend, IMogStoreCommunicator storeCommunicator)
        {
            _trackerBackend = trackerBackend;
            _storeCommunicator = storeCommunicator;
        }

        public void StoreFile(string domain, string key, string storageClass, Stream fileStream, int timeoutMilliseconds)
        {
            var createOpenResponse = _trackerBackend.DoRequest(MogileFsCommands.CREATE_OPEN, new Dictionary<string, string>
                {
                    {"domain", domain},
                    {"key", key},
                    {"class", storageClass},
                });

            if (createOpenResponse == null)
                throw new MogileFsProtocolException(string.Format("Error encounted during CREATE_OPEN: " + _trackerBackend.LastErrStr));

            var uriString = createOpenResponse["path"];
            var uri = new Uri(uriString);

            _storeCommunicator.UploadFile(uri, fileStream, timeoutMilliseconds);

            var fid = createOpenResponse["fid"];
            var devid = createOpenResponse["devid"];

            long totalBytes = fileStream.Length;

            var createCloseResponse = _trackerBackend.DoRequest(MogileFsCommands.CREATE_CLOSE, new Dictionary<string, string>
                {
                    {"fid", fid},
                    {"devid", devid},
                    {"domain", domain},
                    {"size", totalBytes.ToString()},
                    {"key", key},
                    {"path", uriString},
                });

            if (createCloseResponse == null)
                throw new MogileFsProtocolException(string.Format("Error encounted during CREATE_CLOSE: " + _trackerBackend.LastErrStr));
        }

        public byte[] GetFileBytes(string domain, string key, int timeoutMilliseconds)
        {
            var paths = this.GetPaths(domain, key, true);
            if (paths == null || paths.Count == 0)
                throw new NoPathsException(string.Format("No paths found for key: {0}", key));

            var random = new Random();
            var startIndex = random.Next(paths.Count - 1);

            for (int i = 0; i < paths.Count; i++) {
                var path = paths[(i + startIndex) % paths.Count];

                try {
                    return _storeCommunicator.DownloadFile(path, timeoutMilliseconds);
                }
                catch (WebException)
                {
                    // Maybe the mogstored did not respond, try next path
                }
                catch (IOException)
                {
                    // We probably failed to read the whole file, try next path
                }
            }
            throw new StorageCommunicationException(string.Format("Tried to fetch url and all failed. Urls: {0} ", ",".Join(paths)));
        }

        public void Delete(string domain, string key)
        {
            var response = _trackerBackend.DoRequest(MogileFsCommands.DELETE, new Dictionary<string, string>
                {
                    {"domain", domain},
                    {"key", key},
                });
            if (response == null)
                throw new MogileFsProtocolException(string.Format("Error encounted during DELETE: " + _trackerBackend.LastErrStr));
        }

        public void Rename(string domain, string fromKey, string toKey)
        {
            var dict = new Dictionary<string, string>
                {
                    {"domain", domain},
                    {"from_key", fromKey},
                    {"to_key", toKey},
                };

            var response = _trackerBackend.DoRequest(MogileFsCommands.RENAME, dict);
            if (response == null)
                throw new MogileFsProtocolException(string.Format("Error encounted during RENAME: " + _trackerBackend.LastErrStr));
        }

        public void Sleep(int seconds)
        {
            var dict = new Dictionary<string, string>
                {
                    { "duration", seconds.ToString() }
                };

            _trackerBackend.DoRequest(MogileFsCommands.SLEEP, dict);
        }

        public IList<string> GetPaths(string domain, string key, bool noVerify)
        {
            var response = _trackerBackend.DoRequest(MogileFsCommands.GET_PATHS, new Dictionary<string, string>
                {
                    {"domain", domain},
                    {"key", key},
                    {"noverify", noVerify ? "1" : "0"},
                });

            if (response == null)
                return null;

            int pathCount = int.Parse(response["paths"]);
            var retval = new List<string>();
            for (int i = 1; i <= pathCount; i++) {
                retval.Add(response["path" + i]);
            }

            return retval;
        }
    }
}