using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
   // public delegate TValue Getter<in TInstance, out TValue>(TInstance instance);
   // public delegate void Setter<in TInstance, in TValue>(TInstance instance, TValue value);


    public partial class Dynamic
    {
        public static Func<TInstance, TProperty> InstanceGetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return InstanceGetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static Action<TInstance, TProperty> InstanceSetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return InstanceSetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static Func<TProperty> StaticGetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StaticGetProperty<TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static Action<TProperty> StaticSetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StaticSetProperty<TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static ByRefGetter<TInstance, TProperty> StructGetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StructGetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static ByRefSetter<TInstance, TProperty> StructSetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StructSetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }



        public static Func<TInstance, TProperty> InstanceGetProperty<TInstance, TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"get_{property.DeclaringType.Name}_{property.Name}", typeof(TProperty), new Type[] { typeof(TInstance) }, true);
            var method = property.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt, method);
            gen.Emit(OpCodes.Ret);

            return (Func<TInstance, TProperty>)getter.CreateDelegate(typeof(Func<TInstance, TProperty>));
        }

        public static Action<TInstance, TProperty> InstanceSetProperty<TInstance, TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"set_{property.DeclaringType.Name}_{property.Name}", null, new Type[] { typeof(TInstance), typeof(TProperty) }, true);
            var method = property.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, method);
            gen.Emit(OpCodes.Ret);

            return (Action<TInstance, TProperty>)getter.CreateDelegate(typeof(Action<TInstance, TProperty>));
        }

        public static ByRefGetter<TInstance, TProperty> StructGetProperty<TInstance, TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"get_{property.DeclaringType.Name}_{property.Name}", typeof(TProperty), new Type[] { typeof(TInstance).MakeByRefType() }, true);
            var method = property.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return (ByRefGetter<TInstance, TProperty>)getter.CreateDelegate(typeof(ByRefGetter<TInstance, TProperty>));
        }

        public static ByRefSetter<TInstance, TProperty> StructSetProperty<TInstance, TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"set_{property.DeclaringType.Name}_{property.Name}", null, new Type[] { typeof(TInstance).MakeByRefType(), typeof(TProperty) }, true);
            var method = property.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return (ByRefSetter<TInstance, TProperty>)getter.CreateDelegate(typeof(ByRefSetter<TInstance, TProperty>));
        }



        public static Func<TProperty> StaticGetProperty<TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"get_{property.DeclaringType.Name}_{property.Name}", typeof(TProperty), new Type[] { }, true);
            var method = property.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return (Func<TProperty>)getter.CreateDelegate(typeof(Func<TProperty>));
        }

        public static Action<TProperty> StaticSetProperty<TProperty>(PropertyInfo property)
        {
            DynamicMethod getter = new DynamicMethod($"set_{property.DeclaringType.Name}_{property.Name}", null, new Type[] { typeof(TProperty) }, true);
            var method = property.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return (Action<TProperty>)getter.CreateDelegate(typeof(Action<TProperty>));
        }


    }
}
