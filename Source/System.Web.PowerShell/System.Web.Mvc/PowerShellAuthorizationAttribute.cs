using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.PowerShell;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class PowerShellAuthorization : AuthorizeAttribute
    {
        protected PowerShellAuthorization()
        {
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!PowerShellAuthorizationInvoker.IsAuthorized(this, filterContext.HttpContext, filterContext.HttpContext.User, filterContext.ActionDescriptor, filterContext.RouteData.Values.ToDictionary(i => i.Key, i => i.Value)))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public abstract HttpPowerShellCommand ToCommand(object parameters);

        public sealed class ScriptAttribute : PowerShellAuthorization
        {
            string _script;

            public ScriptAttribute(string script)
            {
                this._script = script;
            }

            public override HttpPowerShellCommand ToCommand(object parameters)
            {
                return HttpPowerShellCommand.FromScript(this._script, parameters: parameters, useLocalScope: true);
            }
        }

        public sealed class FileAttribute : PowerShellAuthorization
        {
            string _path;

            public FileAttribute(string path)
            {
                this._path = path;
            }

            public override HttpPowerShellCommand ToCommand(object parameters)
            {
 	            return HttpPowerShellCommand.FromFile(this._path, parameters: parameters, useLocalScope: true);
            }
        }

        public sealed class ResourceAttribute : PowerShellAuthorization
        {
            string _resourceName;
            Func<ResourceManager> _getResourceManager;

            ResourceAttribute(string resourceName)
            {
                this._resourceName = resourceName;
            }

            public ResourceAttribute(ResourceManager manager, string resourceName)
                : this(resourceName)
            {
                _getResourceManager = () => manager;
            }

            public ResourceAttribute(Type resourceSource, string resourceName)
                : this(resourceName)
            {
                _getResourceManager = () => new ResourceManager(resourceSource);
            }

            public ResourceAttribute(string baseName, Assembly assembly, string resourceName)
                : this(resourceName)
            {
                _getResourceManager = () => new ResourceManager(baseName, assembly);
            }

            public override HttpPowerShellCommand ToCommand(object parameters)
            {
                return HttpPowerShellCommand.FromResource(this._getResourceManager(), this._resourceName, parameters: parameters, useLocalScope: true);
            }
        }
    }
}
