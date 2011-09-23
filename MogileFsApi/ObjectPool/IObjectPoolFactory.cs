namespace Primelabs.Twingly.MogileFsApi.ObjectPool
{
    public interface IObjectPoolFactory<T>
    {
        T CreateInstance();
    }
}