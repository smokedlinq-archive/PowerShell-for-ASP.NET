using System.Linq;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class PowerShellAuthorizationAttribute : AuthorizeAttribute
    {
        public PowerShellAuthorizationAttribute()
        {
        }

        public string Script
        {
            get;
            set;
        }

        public string File
        {
            get;
            set;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!PowerShellAuthorizationInvoker.IsAuthorized(this, filterContext.HttpContext, filterContext.HttpContext.User, filterContext.ActionDescriptor, filterContext.RouteData.Values.ToDictionary(i => i.Key, i => i.Value)))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
