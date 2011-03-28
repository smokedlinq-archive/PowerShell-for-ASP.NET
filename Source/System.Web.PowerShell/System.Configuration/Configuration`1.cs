
namespace System.Configuration
{
    public static class Configuration<T>
        where T : IConfiguration
    {
        static T __instance = default(T);
        static object __syncRoot = new object();
        static volatile IConfigurationFactory<T> __factory = new DefaultConfigurationFactory<T>();

        public static IConfigurationFactory<T> Factory 
        {
            get { return __factory; }
            set
            {
                lock (__syncRoot)
                {
                    __factory = value;
                }
            }
        }

        public static T GetConfiguration()
        {
            if (__instance == null)
            {
                lock (__syncRoot)
                {
                    if (__instance == null)
                    {
                        __instance = __factory.CreateInstance();
                    }
                }
            }

            return __instance;
        }
    }
}
