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
        public static Func<TInstance, TField> InstanceGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return InstanceGetField<TInstance, TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }

        public static Action<TInstance, TField> InstanceSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return InstanceSetField<TInstance, TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }


        public static ByRefGetter<TInstance, TField> StructGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StructGetField<TInstance, TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }

        public static ByRefSetter<TInstance, TField> StructSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StructSetField<TInstance, TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }



        public static Func<TField> StaticGetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StaticGetField<TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }

        public static Action<TField> StaticSetField<TInstance, TField>(string fieldName, BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            return StaticSetField<TField>(typeof(TInstance).GetField(fieldName, bindingAttr));
        }



        public static Func<TInstance, TField> InstanceGetField<TInstance, TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { typeof(TInstance) }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);
            gen.Emit(OpCodes.Ret);

            return (Func<TInstance, TField>)getter.CreateDelegate(typeof(Func<TInstance, TField>));
        }

        public static Action<TInstance, TField> InstanceSetField<TInstance, TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TInstance), typeof(TField) }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);

            return (Action<TInstance, TField>)getter.CreateDelegate(typeof(Action<TInstance, TField>));
        }


        public static ByRefGetter<TInstance, TField> StructGetField<TInstance, TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { typeof(TInstance).MakeByRefType() }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);
            gen.Emit(OpCodes.Ret);

            return (ByRefGetter<TInstance, TField>)getter.CreateDelegate(typeof(ByRefGetter<TInstance, TField>));
        }

        public static ByRefSetter<TInstance, TField> StructSetField<TInstance, TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TInstance).MakeByRefType(), typeof(TField) }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);

            return (ByRefSetter<TInstance, TField>)getter.CreateDelegate(typeof(ByRefSetter<TInstance, TField>));
        }




        public static Func<TField> StaticGetField<TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"get_{field.DeclaringType.Name}_{field.Name}", typeof(TField), new Type[] { }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, field);
            gen.Emit(OpCodes.Ret);

            return (Func<TField>)getter.CreateDelegate(typeof(Func<TField>));
        }

        public static Action<TField> StaticSetField<TField>(FieldInfo field)
        {
            DynamicMethod getter = new DynamicMethod($"set_{field.DeclaringType.Name}_{field.Name}", null, new Type[] { typeof(TField) }, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, field);
            gen.Emit(OpCodes.Ret);

            return (Action<TField>)getter.CreateDelegate(typeof(Action<TField>));
        }


    }
}
