using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    static class MemberHandler
    {
        public static bool CanWrite(this MemberInfo member)
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

        public static bool CanRead(this MemberInfo member)
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

        public static Type MemberType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Method:
                    return DelegateType((MethodInfo)member);
                default:
                    return null;
            }

        }

        public static object GetValue(this MemberInfo member, object obj)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(obj);
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetValue(obj);
                case MemberTypes.Method:
                    return CreateDelegate((MethodInfo)member, obj);
                default:
                    return null;
            }
        }

        private static Type DelegateType(MethodInfo member)
        {
            var tArgs = new List<Type>();
            foreach (var param in member.GetParameters())
                tArgs.Add(param.ParameterType);
            tArgs.Add(member.ReturnType);
            return Expression.GetDelegateType(tArgs.ToArray());
        }

        private static object CreateDelegate(MethodInfo member, object obj)
        {
            if (((MethodInfo)member).IsStatic)
            {
                return Delegate.CreateDelegate(DelegateType(member),member);
            }
            else
            {
                return Delegate.CreateDelegate(DelegateType(member), obj, member);
            }
        }

        static public bool IsStatic(this MemberInfo member)
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

        static public void SetValue(this MemberInfo member, object obj, object value)
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


    }


    static class GenReflection
    {
        /*
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
        }*/

        static Dictionary<MemberInfo, Delegate> SettersCache = new Dictionary<MemberInfo, Delegate>();
        static Dictionary<MemberInfo, Delegate> GettersCache = new Dictionary<MemberInfo, Delegate>();

        public static Func<object, T> GetGetValueDelegate<T>(this MemberInfo member)
        {
            lock (GettersCache)
            {
                Delegate getter;
                if (!GettersCache.TryGetValue(member, out getter))
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Property:
                            getter = CreateGetPropertyValueDelegate<T>((PropertyInfo)member);
                            break;
                        case MemberTypes.Field:
                            getter = CreateGetFieldValueDelegate<T>((FieldInfo)member);
                            break;
                    }
                    GettersCache[member] = getter;
                }
                return (Func<object, T>)getter;
            }
        }

        /// <summary>
        /// Constructs propperty getter wrapper method for provided property info
        /// </summary>
        /// <typeparam name="T">Type of the property, should match prop.PropertyType</typeparam>
        /// <param name="prop">The property getter wrapper is constructed for</param>
        /// <returns>The getter wrapper method</returns>
        /// <remarks>Created getter wrapper is bound to type of object owning the prorerty. Func object argument must be of this type or of one of its subclasses</remarks>
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

        /// <summary>
        /// Constructs field getter method for provided field info
        /// </summary>
        /// <typeparam name="T">Type of the field, should match field.FieldType</typeparam>
        /// <param name="field">The field getter is constructed for</param>
        /// <returns>The getter method</returns>
        /// <remarks>Created getter is bound to type of object owning the field. Func object argument must be of this type or of one of its subclasses</remarks>
        private static Func<object, T> CreateGetFieldValueDelegate<T>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(T), new Type[] { typeof(object) }, true);
            ILGenerator gen = getter.GetILGenerator();


            if (field.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, field);
                gen.Emit(OpCodes.Ret);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ret);
            }

            return (Func<object, T>)getter.CreateDelegate(typeof(Func<object, T>));
        }

        public static Action<object, T> GetSetValueDelegate<T>(this MemberInfo member)
        {
            lock (SettersCache)
            {
                Delegate setter;
                if (!SettersCache.TryGetValue(member, out setter))
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Property:
                            setter = CreateSetPropertyValueDelegate<T>((PropertyInfo)member);
                            break;
                        case MemberTypes.Field:
                            setter = CreateSetFieldValueDelegate<T>((FieldInfo)member);
                            break;
                    }
                    SettersCache[member] = setter;
                }
                return (Action<object, T>)setter;
            }
        }

        /// <summary>
        /// Constructs propperty setter wrapper method for provided property info
        /// </summary>
        /// <typeparam name="T">Type of the property, should match prop.PropertyType</typeparam>
        /// <param name="prop">The property setter wrapper is constructed for</param>
        /// <returns>The setter wrapper method</returns>
        /// <remarks>Created setter wrapper is bound to type of object owning the prorerty. Action object argument must be of this type or of one of its subclasses</remarks>
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

        /// <summary>
        /// Constructs field setter method for provided field info
        /// </summary>
        /// <typeparam name="T">type of the field, should match field.FieldType</typeparam>
        /// <param name="field">the field getter is constructed for</param>
        /// <returns>the setter method</returns>
        /// <remarks>Created setter is bound to type of object owning the field. Action object argument must be of this type or of one of its subclasses</remarks>
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

        public static Type GetRWType(string typeName)
        {
            const string ns = "RWLayout.alpha2";
            Type valueType = GenTypes.GetTypeInAnyAssembly(typeName, ns);
            valueType ??= GenTypes.GetTypeInAnyAssembly($"{ns}.{typeName}", ns);
            return valueType;
        }
    }
}
