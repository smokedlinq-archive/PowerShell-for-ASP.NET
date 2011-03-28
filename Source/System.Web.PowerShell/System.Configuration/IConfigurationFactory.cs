
namespace System.Configuration
{
    public interface IConfigurationFactory<T>
        where T : IConfiguration
    {
        T CreateInstance();
    }
}
