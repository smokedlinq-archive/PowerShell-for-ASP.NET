using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.PowerShell;

namespace System.Web.Mvc
{
    public static class PowerShellAuthorization
    {
        public static bool IsAuthorized(string scriptPath, HttpContextBase context, IPrincipal principal, ActionDescriptor action, object routeValues, bool test = false)
        {
            var parameters = new 
                { 
                    HttpContext = context, 
                    Principal = principal, 
                    Action = action, 
                    RouteValues = routeValues, 
                    Test = test 
                };

            return HttpPowerShell.Invoke<bool>(scriptPath, parameters: parameters, isScript: true).FirstOrDefault();
        }

        public static bool IsAuthorized<T>(this T controller, Expression<Action<T>> action, object routeValues = null)
            where T : Controller
        {
            var actionDesciptor = GetActionDescriptor<T>(action);
            var scripts = GetAuthorizationScripts(actionDesciptor);

            foreach (var scriptPath in scripts)
            {
                if (!IsAuthorized(scriptPath, controller.HttpContext, controller.User, actionDesciptor, routeValues, true))
                {
                    return false;
                }
            }

            return true;
        }

        static ActionDescriptor GetActionDescriptor<T>(Expression<Action<T>> action)
            where T : Controller
        {
            var call = action.Body as MethodCallExpression;

            if (call == null)
            {
                throw new ArgumentException("The action parameter must call a method.");
            }

            var controllerDescriptor = new ReflectedControllerDescriptor(typeof(T));

            return new ReflectedActionDescriptor(call.Method, call.Method.Name, controllerDescriptor);
        }

        static IEnumerable<string> GetAuthorizationScripts(ActionDescriptor action)
        {
            return action.ControllerDescriptor.GetCustomAttributes(typeof(PowerShellAuthorizationAttribute), true)
                .Union(action.GetCustomAttributes(typeof(PowerShellAuthorizationAttribute), false))
                .Cast<PowerShellAuthorizationAttribute>()
                .Select(attr => attr.ScriptPath);
        }
    }
}
