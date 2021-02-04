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
        private MemberInfo member = null;

        public MemberHandler(MemberInfo member)
        {
            this.member = member;
        }

        public bool CanWrite
        {
            get
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        return true;
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).CanWrite;
                    case MemberTypes.Method:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public Type MemberType
        {
            get
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        return ((FieldInfo)member).FieldType;
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).PropertyType;
                    case MemberTypes.Method:
                        return DelegateType;
                    default:
                        return null;
                }
            }
        }

        internal object GetValue(object obj)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(obj);
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetValue(obj);
                case MemberTypes.Method:
                    return CreateDelegate(obj);
                default:
                    return null;
            }
        }

        Type delegateType = null;
        private Type DelegateType
        {
            get
            {
                if (delegateType == null)
                {
                    var tArgs = new List<Type>();
                    foreach (var param in ((MethodInfo)member).GetParameters())
                        tArgs.Add(param.ParameterType);
                    tArgs.Add(((MethodInfo)member).ReturnType);
                    delegateType = System.Linq.Expressions.Expression.GetDelegateType(tArgs.ToArray());
                }
                return delegateType;
            }
        }

        private object CreateDelegate(object obj)
        {
            var delDecltype = DelegateType;
            if (((MethodInfo)member).IsStatic) {
                return Delegate.CreateDelegate(delDecltype, ((MethodInfo)member));
            }
            else
            {
                return Delegate.CreateDelegate(delDecltype, obj, ((MethodInfo)member));
            }
        }

        internal void SetValue(object obj, object value)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(obj, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(obj, value);
                    break;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}:{{{member}}}";
        }
    }

    static class GenReflection
    {
        public static MemberHandler GetMemberHandler(this Type type, string name, BindingFlags bindingAttr)
        {
            var member = type.GetMember(name, bindingAttr)?.FirstOrDefault();
            if (member == null)
            {
                return null;
            }

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                case MemberTypes.Method:
                    return new MemberHandler(member);
                default:
                    return null;
            }
        }
    }
}
