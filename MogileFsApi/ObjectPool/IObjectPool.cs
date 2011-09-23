namespace Primelabs.Twingly.MogileFsApi.ObjectPool
{
    public interface IObjectPool<T>
    {
        ObjectPoolHandle<T> Borrow();
        void Return(ObjectPoolHandle<T> handle);
    }
}