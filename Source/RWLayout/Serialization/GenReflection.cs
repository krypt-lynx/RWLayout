using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    class MemberHandler
    {
        private PropertyInfo prop = null;
        private FieldInfo field = null;
        private MethodInfo method = null;

        public MemberHandler(PropertyInfo prop)
        {
            this.prop = prop;
        }

        public MemberHandler(FieldInfo field)
        {
            this.field = field;
        }

        public MemberHandler(MethodInfo member)
        {
            this.method = member;
        }

        public bool CanWrite
        {
            get
            {
                return prop?.CanWrite ?? (field != null);
            }
        }

        public Type MemberType
        {
            get
            {
                return prop?.PropertyType ?? field?.FieldType ?? null;
            }
        }

        internal object GetValue(object obj)
        {
            if (prop != null)
            {
                return prop.GetValue(obj);
            } 
            else if (field != null)
            {
                return field.GetValue(obj);
            }
            else if (method != null)
            {
                return CreateDelegate(obj);
            }
            else
            {
                return null;
            }
        }

        Type delegateType = null;
        private Type DelegateType()
        {
            if (delegateType == null)
            {
                var tArgs = new List<Type>();
                foreach (var param in method.GetParameters())
                    tArgs.Add(param.ParameterType);
                tArgs.Add(method.ReturnType);
                delegateType = System.Linq.Expressions.Expression.GetDelegateType(tArgs.ToArray());
            }
            return delegateType;
        }

        private object CreateDelegate(object obj)
        {
            var delDecltype = DelegateType();
            if (method.IsStatic) {
                return Delegate.CreateDelegate(delDecltype, method);
            }
            else
            {
                return Delegate.CreateDelegate(delDecltype, obj, method);
            }
        }

        internal void SetValue(object obj, object value)
        {
            if (prop != null)
            {
                prop.SetValue(obj, value);
            }
            else if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }

    static class GenReflection
    {
        public static MemberHandler GetMemberHandler(this Type type, string name, BindingFlags bindingAttr)
        {
            var member = type.GetMember(name, bindingAttr).First(); // todo
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return new MemberHandler((FieldInfo)member);
                case MemberTypes.Property:
                    return new MemberHandler((PropertyInfo)member);
                case MemberTypes.Method:
                    return new MemberHandler((MethodInfo)member);
                default:
                    return null;
            }
        }
    }
}
