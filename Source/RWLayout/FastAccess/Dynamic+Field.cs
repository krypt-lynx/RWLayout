using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    public partial class Dynamic
    {
        public static Getter<TInstance, TField> InstanceGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return InstanceGetField<TInstance, TField>(field);
        }

        public static Setter<TInstance, TField> InstanceSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : class
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return InstanceSetField<TInstance, TField>(field);
        }


        public static ByRefGetter<TInstance, TField> StructGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return StructGetField<TInstance, TField>(field);
        }

        public static ByRefSetter<TInstance, TField> StructSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where TInstance : struct
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return StructSetField<TInstance, TField>(field);
        }



        public static Func<TField> StaticGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return StaticGetField<TField>(field);
        }

        public static Action<TField> StaticSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var field = typeof(TInstance).GetField(fieldName, bindingAttr);
            if (field == null)
            {
                throw new MemberNotFoundException(fieldName, typeof(TInstance));
            }
            return StaticSetField<TField>(field);
        }



        public static Getter<TInstance, TField> InstanceGetField<TInstance, TField>(FieldInfo field) where TInstance : class
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { typeof(TInstance) }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldarg_0()
                .Ldfld(field)
                .Ret();

            return (Getter<TInstance, TField>)getter.CreateDelegate(typeof(Getter<TInstance, TField>));
        }

        public static Setter<TInstance, TField> InstanceSetField<TInstance, TField>(FieldInfo field) where TInstance : class
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TInstance), typeof(TField) }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldarg_0()
                .Ldarg_1()
                .Stfld(field)
                .Ret();

            return (Setter<TInstance, TField>)getter.CreateDelegate(typeof(Setter<TInstance, TField>));
        }


        public static ByRefGetter<TInstance, TField> StructGetField<TInstance, TField>(FieldInfo field) where TInstance : struct
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { typeof(TInstance).MakeByRefType() }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldarg_0()
                .Ldfld(field)
                .Ret();

            return (ByRefGetter<TInstance, TField>)getter.CreateDelegate(typeof(ByRefGetter<TInstance, TField>));
        }

        public static ByRefSetter<TInstance, TField> StructSetField<TInstance, TField>(FieldInfo field) where TInstance : struct
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TInstance).MakeByRefType(), typeof(TField) }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldarg_0()
                .Ldarg_1()
                .Stfld(field)
                .Ret();

            return (ByRefSetter<TInstance, TField>)getter.CreateDelegate(typeof(ByRefSetter<TInstance, TField>));
        }




        public static Func<TField> StaticGetField<TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldsfld(field)
                .Ret();

            return (Func<TField>)getter.CreateDelegate(typeof(Func<TField>));
        }

        public static Action<TField> StaticSetField<TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TField) }, typeof(Dynamic), true);
            IILGenerator gen = getter.GetILGenerator().AsInterface();

            gen
                .Ldarg_0()
                .Stsfld(field)
                .Ret();

            return (Action<TField>)getter.CreateDelegate(typeof(Action<TField>));
        }


    }
}
