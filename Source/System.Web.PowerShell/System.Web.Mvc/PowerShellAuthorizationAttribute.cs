using System.Linq;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PowerShellAuthorizationAttribute : AuthorizeAttribute
    {
        public PowerShellAuthorizationAttribute(string scriptPath)
        {
            this.ScriptPath = scriptPath;
        }

        public string ScriptPath
        {
            get;
            private set;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!PowerShellAuthorization.IsAuthorized(this.ScriptPath, filterContext.HttpContext, filterContext.HttpContext.User, filterContext.ActionDescriptor, filterContext.RouteData.Values.ToDictionary(i => i.Key, i => i.Value)))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
