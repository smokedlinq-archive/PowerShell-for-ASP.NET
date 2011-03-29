using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    internal sealed class DynamicPSObject : DynamicObject
    {
        public DynamicPSObject(PSObject obj)
        {
            this.BaseObject = obj;
        }

        public PSObject BaseObject
        {
            get;
            private set;
        }

        public static explicit operator DynamicPSObject(PSObject obj)
        {
            return obj == null ? null : new DynamicPSObject(obj);
        }

        public static explicit operator PSObject(DynamicPSObject obj)
        {
            return obj == null ? null : obj.BaseObject;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = this.BaseObject.Members[binder.Name] as PSPropertyInfo;

            if (property == null || !property.IsGettable)
            {
                result = null;
                return false;
            }

            result = property.Value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = this.BaseObject.Members[binder.Name] as PSPropertyInfo;

            if (property == null || !property.IsSettable)
            {
                return false;
            }

            property.Value = value;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var method = this.BaseObject.Members[binder.Name] as PSMethodInfo;

            if (method == null)
            {
                result = null;
                return false;
            }

            result = method.Invoke(args);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.BaseObject.Members.Select(i => i.Name);
        }
    }
}
