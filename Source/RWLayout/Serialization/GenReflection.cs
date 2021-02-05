using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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

        public bool CanRead
        {
            get
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        return true;
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).CanRead;
                    case MemberTypes.Method:
                        return true;
                    default:
                        return false;
                }
            }
        }


        public Type TargetType
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

        public MemberInfo Member => member;

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
                    delegateType = Expression.GetDelegateType(tArgs.ToArray());
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

        public bool IsStatic
        {
            get
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        return ((FieldInfo)member).IsStatic;
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).GetAccessors().FirstOrDefault()?.IsStatic ?? false;
                    case MemberTypes.Event: // whatever, just making it to semiwork for unsupported cases
                        return ((EventInfo)member).GetAddMethod()?.IsStatic ?? false;
                    case MemberTypes.Method:
                    case MemberTypes.Constructor:
                        return ((MethodBase)member).IsStatic;

                    default:
                        return false;
                }
            }
        }

        public void SetValue(object obj, object value)
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

        static Dictionary<MemberInfo, Delegate> SettersCache = new Dictionary<MemberInfo, Delegate>();
        static Dictionary<MemberInfo, Delegate> GettersCache = new Dictionary<MemberInfo, Delegate>();

        public static Func<object, T> GetGetValueDelegate<T>(this MemberHandler member)
        {
            lock (GettersCache)
            {
                Delegate getter;
                if (!GettersCache.TryGetValue(member.Member, out getter))
                {
                    switch (member.Member.MemberType)
                    {
                        case MemberTypes.Property:
                            getter = CreateGetPropertyValueDelegate<T>((PropertyInfo)member.Member);
                            break;
                        case MemberTypes.Field:
                            getter = CreateGetFieldValueDelegate<T>((FieldInfo)member.Member);
                            break;
                    }
                    GettersCache[member.Member] = getter;
                }
                return (Func<object, T>)getter;
            }
        }

        private static Func<object, T> CreateGetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            DynamicMethod getter = new DynamicMethod($"get_{prop.DeclaringType.Name}_{prop.Name}", typeof(T), new Type[] { typeof(object) }, true);
            MethodInfo method = prop.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }

            return (Func<object, T>)getter.CreateDelegate(typeof(Func<object, T>));
        }

        private static Func<object, T> CreateGetFieldValueDelegate<T>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(T), new Type[] { typeof(object) }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, field.DeclaringType);
            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Ldfld, field);
            }
            gen.Emit(OpCodes.Ret);

            return (Func<object, T>)getter.CreateDelegate(typeof(Func<object, T>));
        }

        public static Action<object, T> GetSetValueDelegate<T>(this MemberHandler member)
        {
            lock (SettersCache)
            {
                Delegate setter;
                if (!SettersCache.TryGetValue(member.Member, out setter))
                {
                    switch (member.Member.MemberType)
                    {
                        case MemberTypes.Property:
                            setter = CreateSetPropertyValueDelegate<T>((PropertyInfo)member.Member);
                            break;
                        case MemberTypes.Field:
                            setter = CreateSetFieldValueDelegate<T>((FieldInfo)member.Member);
                            break;
                    }
                    SettersCache[member.Member] = setter;
                }
                return (Action<object, T>)setter;
            }
        }

        private static Action<object, T> CreateSetPropertyValueDelegate<T>(PropertyInfo prop)
        {
            DynamicMethod getter = new DynamicMethod($"set_{prop.DeclaringType.Name}_{prop.Name}", null, new Type[] { typeof(object), typeof(T) }, true);
            MethodInfo method = prop.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            if (method.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Call, method);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, method);
                gen.Emit(OpCodes.Ret);
            }

            return (Action<object, T>)getter.CreateDelegate(typeof(Action<object, T>));
        }

        private static Action<object, T> CreateSetFieldValueDelegate<T>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(object), typeof(T) }, true);
            ILGenerator gen = getter.GetILGenerator();

            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, field);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, field);
                gen.Emit(OpCodes.Ret);
            }

            return (Action<object, T>)getter.CreateDelegate(typeof(Action<object, T>));
        }
    }
}
