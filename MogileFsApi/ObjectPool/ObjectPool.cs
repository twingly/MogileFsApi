using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Primelabs.Twingly.MogileFsApi.ObjectPool
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        private readonly object _syncRoot = new object();
        private readonly Queue<ObjectPoolHandle<T>> _returnedObjects;
        private readonly Dictionary<string, ObjectPoolHandle<T>> _borrowedObjects;
        private readonly IObjectPoolFactory<T> _factory;

        public ObjectPool(IObjectPoolFactory<T> factory)
        {
            _returnedObjects = new Queue<ObjectPoolHandle<T>>();
            _borrowedObjects = new Dictionary<string, ObjectPoolHandle<T>>();
            _factory = factory;
        }

        public ObjectPool(InstanceCreator<T> creator)
            : this(new GenericObjectPoolFactory<T>(creator))
        {
        }

        public ObjectPoolHandle<T> Borrow()
        {
            lock (_syncRoot) {
                ObjectPoolHandle<T> retval;
                if (_returnedObjects.Count > 0) {
                    retval = _returnedObjects.Dequeue();
                } else {
                    var instance = _factory.CreateInstance();
                    retval = new ObjectPoolHandle<T>(this, instance);
                }
                retval.Invalidate = true;
                _borrowedObjects.Add(retval.Guid.ToString(), retval);
                return retval;
            }
        }

        public void Return(ObjectPoolHandle<T> handle)
        {
            lock (_syncRoot)
            {
                if (! _borrowedObjects.ContainsKey(handle.Guid.ToString()))
                    throw new IndexOutOfRangeException(string.Format("The handle with guid {0} is not borrowed!", handle.Guid.ToString()));

                _borrowedObjects.Remove(handle.Guid.ToString());

                // If user actively set invalidate to false, we return it to the pool.
                if (!handle.Invalidate)
                    _returnedObjects.Enqueue(handle);
                else {
                    if (handle.Instance is IDisposable) {
                        ((IDisposable) handle.Instance).Dispose();
                    }
                }
            }            
        }
    }
}
