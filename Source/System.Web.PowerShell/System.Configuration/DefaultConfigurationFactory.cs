
namespace System.Configuration
{
    public class DefaultConfigurationFactory<T> : IConfigurationFactory<T>
        where T : IConfiguration
    {
        public T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
