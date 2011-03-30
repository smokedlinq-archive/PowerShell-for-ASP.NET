using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.PowerShell;

namespace System.Web.Mvc
{
    public static class PowerShellAuthorizationInvoker
    {
        internal static bool IsAuthorized(PowerShellAuthorization attribute, HttpContextBase context, IPrincipal principal, ActionDescriptor action, object routeValues, bool test = false)
        {
            var parameters = new 
                { 
                    HttpContext = context, 
                    Principal = principal, 
                    Action = action, 
                    RouteValues = routeValues, 
                    Test = test 
                };

            return attribute.ToCommand(parameters).Invoke<bool>().FirstOrDefault();
        }

        public static bool IsAuthorized<T>(this T controller, Expression<Action<T>> action)
            where T : Controller
        {
            var actionDesciptor = GetActionDescriptor<T>(action);
            var attributes = GetAuthorizationAttributes(actionDesciptor);
            var routeValues = GetRouteValues(action);
    
            foreach (var attr in attributes)
            {
                if (!IsAuthorized(attr, controller.HttpContext, controller.User, actionDesciptor, routeValues, true))
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

        static IDictionary<string, object> GetRouteValues<T>(Expression<Action<T>> action)
            where T : Controller
        {
            var call = action.Body as MethodCallExpression;

            if (call == null)
            {
                throw new ArgumentException("The action parameter must call a method.");
            }

            var routeValues = new Dictionary<string, object>();
            var parameters = call.Method.GetParameters();
            var arguments = call.Arguments;

            for (int i=0; i<parameters.Length; i++)
            {
                routeValues.Add(parameters[i].Name, Expression.Lambda(arguments[i]).Compile().DynamicInvoke());
            }

            return routeValues;
        }

        static IEnumerable<PowerShellAuthorization> GetAuthorizationAttributes(ActionDescriptor action)
        {
            return action.ControllerDescriptor.GetCustomAttributes(typeof(PowerShellAuthorization), true)
                .Union(action.GetCustomAttributes(typeof(PowerShellAuthorization), false))
                .Cast<PowerShellAuthorization>();
        }
    }
}
