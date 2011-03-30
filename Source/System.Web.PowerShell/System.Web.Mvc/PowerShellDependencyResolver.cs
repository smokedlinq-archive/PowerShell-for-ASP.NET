using System.Collections.Generic;
using System.Linq;
using System.Web.PowerShell;

namespace System.Web.Mvc
{
    public class PowerShellDependencyResolver : IDependencyResolver
    {
        Dictionary<Type, HttpPowerShellCommand> _commands = new Dictionary<Type, HttpPowerShellCommand>();

        public void Register<T>(HttpPowerShellCommand command)
        {
            _commands.Add(typeof(T), command);
        }

        public object GetService(Type serviceType)
        {
            var registeredType = _commands.Keys.Where(t => serviceType.IsAssignableFrom(t)).FirstOrDefault();

            if (registeredType == null)
            {
                return null;
            }

            return _commands[registeredType].Invoke<object>(new Type[] { serviceType }).FirstOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var registeredType = _commands.Keys.Where(t => serviceType.IsAssignableFrom(t)).FirstOrDefault();

            if (registeredType == null)
            {
                return new object[0];
            }

            return _commands[registeredType].Invoke<object>(new Type[] { serviceType });
        }
    }
}
