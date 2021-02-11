﻿using System;
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
        public static Getter<TInstance, TProperty> InstanceGetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            return InstanceGetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static Setter<TInstance, TProperty> InstanceSetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
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

        public static ByRefGetter<TInstance, TProperty> StructGetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            return StructGetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }

        public static ByRefSetter<TInstance, TProperty> StructSetProperty<TInstance, TProperty>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            return StructSetProperty<TInstance, TProperty>(typeof(TInstance).GetProperty(fieldName, bindingAttr));
        }



        public static Getter<TInstance, TProperty> InstanceGetProperty<TInstance, TProperty>(PropertyInfo property) where TInstance : class
        {
            DynamicMethod getter = new DynamicMethod($"get_{property.DeclaringType.Name}_{property.Name}", typeof(TProperty), new Type[] { typeof(TInstance) }, true);
            var method = property.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt, method);
            gen.Emit(OpCodes.Ret);

            return (Getter<TInstance, TProperty>)getter.CreateDelegate(typeof(Getter<TInstance, TProperty>));
        }

        public static Setter<TInstance, TProperty> InstanceSetProperty<TInstance, TProperty>(PropertyInfo property) where TInstance : class
        {
            DynamicMethod getter = new DynamicMethod($"set_{property.DeclaringType.Name}_{property.Name}", null, new Type[] { typeof(TInstance), typeof(TProperty) }, true);
            var method = property.GetSetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, method);
            gen.Emit(OpCodes.Ret);

            return (Setter<TInstance, TProperty>)getter.CreateDelegate(typeof(Setter<TInstance, TProperty>));
        }

        public static ByRefGetter<TInstance, TProperty> StructGetProperty<TInstance, TProperty>(PropertyInfo property) where TInstance : struct
        {
            DynamicMethod getter = new DynamicMethod($"get_{property.DeclaringType.Name}_{property.Name}", typeof(TProperty), new Type[] { typeof(TInstance).MakeByRefType() }, true);
            var method = property.GetGetMethod(true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);

            return (ByRefGetter<TInstance, TProperty>)getter.CreateDelegate(typeof(ByRefGetter<TInstance, TProperty>));
        }

        public static ByRefSetter<TInstance, TProperty> StructSetProperty<TInstance, TProperty>(PropertyInfo property) where TInstance : struct
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
