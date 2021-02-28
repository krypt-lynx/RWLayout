using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{    
    class PassthroughILBuilder : ILArgBuilder
    {
        public PassthroughILBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType) : base(gen, delegateArgType, methodArgType) { }

        public override void PassArg(byte argIndex)
        {
            gen.EmitLdarg(argIndex);
        }

        public static bool WillHandle(Type delegateArgType, Type methodArgType)
        {
            return delegateArgType == methodArgType;
        }
    }

    class CastILBuilder : ILArgBuilder
    {
        public CastILBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType) : base(gen, delegateArgType, methodArgType) { }

        public override void PassArg(byte argIndex)
        {
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Castclass, methodArgType);
        }

        public static bool WillHandle(Type delegateArgType, Type methodArgType)
        {
            return
                !methodArgType.IsValueType &&
                !delegateArgType.IsValueType &&
                !methodArgType.IsByRef &&
                !delegateArgType.IsByRef;
        }
    }

    class UnboxILBuilder : ILArgBuilder
    {
        public UnboxILBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType) : base(gen, delegateArgType, methodArgType) { }

        public override void PassArg(byte argIndex)
        {
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Unbox_Any, methodArgType);
        }

        public static bool WillHandle(Type delegateArgType, Type methodArgType)
        {
            return
                methodArgType.IsValueType &&
                !methodArgType.IsByRef &&
                delegateArgType == typeof(object);
        }
    }

    class ByRefClassILBuilder : ILArgBuilder
    {
        public ByRefClassILBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType) : base(gen, delegateArgType, methodArgType)
        {
            elementType = methodArgType.GetElementType();
        }

        LocalBuilder local;
        Type elementType;

        public override void Prepare(byte argIndex)
        {
            local = gen.DeclareLocal(elementType);
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Castclass, elementType);
            gen.Emit(OpCodes.Stloc, local);
        }

        public override void PassArg(byte argIndex)
        {
            gen.Emit(OpCodes.Ldloca, local);
        }

        public override void Finalize_(byte argIndex)
        {
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Stind_Ref);
        }

        public static bool WillHandle(Type delegateArgType, Type methodArgType)
        {
            return
                methodArgType.IsByRef &&
                delegateArgType.IsByRef &&
                !methodArgType.GetElementType().IsValueType &&
                !delegateArgType.GetElementType().IsValueType;
        }
    }

    class ByRefValueTypeILBuilder : ILArgBuilder
    {
        public ByRefValueTypeILBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType) : base(gen, delegateArgType, methodArgType)
        {
            elementType = methodArgType.GetElementType();
        }

        LocalBuilder local;
        Type elementType;

        public override void Prepare(byte argIndex)
        {
            local = gen.DeclareLocal(elementType);
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Ldind_Ref);
            gen.Emit(OpCodes.Unbox_Any, elementType);
            gen.Emit(OpCodes.Stloc, local);
        }

        public override void PassArg(byte argIndex)
        {
            gen.Emit(OpCodes.Ldloca, local);
        }

        public override void Finalize_(byte argIndex)
        {
            gen.EmitLdarg(argIndex);
            gen.Emit(OpCodes.Ldloc, local);
            gen.Emit(OpCodes.Box, elementType);
            gen.Emit(OpCodes.Stind_Ref);
        }

        public static bool WillHandle(Type delegateArgType, Type methodArgType)
        {
            return
                methodArgType.IsByRef &&
                delegateArgType.IsByRef &&
                methodArgType.GetElementType().IsValueType &&
                delegateArgType.GetElementType() == typeof(object);
        }
    }
    
    abstract class ILArgBuilder
    {
        protected TestILGenerator gen;
        protected Type delegateArgType;
        protected Type methodArgType;

        public ILArgBuilder(TestILGenerator gen, Type delegateArgType, Type methodArgType)
        {
            this.gen = gen;
            this.delegateArgType = delegateArgType;
            this.methodArgType = methodArgType;
        }

        public virtual void Prepare(byte argIndex) { }
        public virtual void PassArg(byte argIndex) { }
        public virtual void Finalize_(byte argIndex) { }

        //public abstract bool WillHandle(Type delegateArgType, Type methodArgType);
    }
}
