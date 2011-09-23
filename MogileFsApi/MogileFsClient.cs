using System;
using System.Collections.Generic;
using System.IO;
using Primelabs.Twingly.MogileFsApi.ObjectPool;

namespace Primelabs.Twingly.MogileFsApi
{
    public class MogileFsClient : IMogileFs
    {
        private readonly ObjectPool<MogileFsProtocolImplementation> _pool;

        public MogileFsClient(Uri[] trackerUris)
        {

            _pool =
                new ObjectPool<MogileFsProtocolImplementation>(
                    () => new MogileFsProtocolImplementation(new TrackerBackend(trackerUris), new MogStoreCommunicator()));
        }

        public void StoreFile(string domain, string key, string storageClass, Stream fileStream, int timeoutMilliseconds)
        {
            using (var proto = _pool.Borrow()) {
                proto.Instance.StoreFile(domain, key, storageClass, fileStream, timeoutMilliseconds);
                // Invalidate gets set to true when we borrow the object from the pool.
                // In order to not recreate objects we set invalidate to false here, 
                // so it can be reused. This "strange" way of doing it (the alternative 
                // would be calling Pool.InvalidateObject()), means that we get some safeness
                // should someone forget to call it.
                proto.Invalidate = false; 
            }
        }

        public byte[] GetFileBytes(string domain, string key, int timeoutMilliseconds)
        {
            using (var proto = _pool.Borrow())
            {
                var retval = proto.Instance.GetFileBytes(domain, key, timeoutMilliseconds);
                proto.Invalidate = false;
                return retval;
            }
        }

        public void Delete(string domain, string key)
        {
            using (var proto = _pool.Borrow())
            {
                proto.Instance.Delete(domain, key);
                proto.Invalidate = false;
            }
        }

        public void Rename(string domain, string fromKey, string toKey)
        {
            using (var proto = _pool.Borrow())
            {
                proto.Instance.Rename(domain, fromKey, toKey);
                proto.Invalidate = false;
            }            
        }

        public void Sleep(int seconds)
        {
            using (var proto = _pool.Borrow())
            {
                proto.Instance.Sleep(seconds);
                proto.Invalidate = false;
            }
        }

        public IList<string> GetPaths(string domain, string key, bool noVerify)
        {
            using (var proto = _pool.Borrow())
            {
                var retval = proto.Instance.GetPaths(domain, key, noVerify);
                proto.Invalidate = false;
                return retval;
            }
        }
    }
}