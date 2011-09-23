using System;

namespace Primelabs.Twingly.MogileFsApi.ObjectPool
{
    public class ObjectPoolHandle<T> : IDisposable
    {
        private readonly IObjectPool<T> _pool;

        public ObjectPoolHandle(IObjectPool<T> pool, T instance)
        {
            _pool = pool;
            Instance = instance;
            Guid = Guid.NewGuid();
        }

        public T Instance { get; private set; }
        public Guid Guid { get; private set;}
        public bool Invalidate { get; set; }

        private bool _isDisposd;
        public void Dispose()
        {
            if (!_isDisposd) {
                _isDisposd = true;
                _pool.Return(this);
            }
        }
    }
}