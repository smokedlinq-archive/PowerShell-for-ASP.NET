using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Dynamic
{
    public static class DynamicConverter
    {
        public static dynamic ToExpando<T>(this T obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is ExpandoObject)
            {
                return obj as ExpandoObject;
            }

            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            if (obj is IDictionary<string, object>)
            {
                var oad = (IDictionary<string, object>)obj;

                foreach (var key in oad.Keys)
                {
                    dictionary.Add(key, oad[key]);
                }
            }
            else if (obj is NameValueCollection)
            {
                var nvc = obj as NameValueCollection;

                foreach (var key in nvc.AllKeys)
                {
                    dictionary.Add(key, nvc[key]);
                }
            }
            else
            {
                foreach (var property in obj.GetType().GetProperties())
                {
                    dictionary.Add(property.Name, property.GetValue(obj, null));
                }
            }

            return expando;
        }

        public static IEnumerable<dynamic> ToExpando<T>(this IEnumerable<T> collection)
        {
            return collection.Select(obj => obj.ToExpando());
        }

        public static IDictionary<string, object> ToDictionary<T>(this T obj)
        {
            return (IDictionary<string, object>)obj.ToExpando();
        }
    }
}
