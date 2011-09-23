namespace Primelabs.Twingly.MogileFsApi.ObjectPool
{
    public delegate T InstanceCreator<T>();

    public class GenericObjectPoolFactory<T> : IObjectPoolFactory<T>
    {
        private readonly InstanceCreator<T> _instanceCreator;

        public GenericObjectPoolFactory(InstanceCreator<T> instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public T CreateInstance()
        {
            return _instanceCreator();
        }
    }
}