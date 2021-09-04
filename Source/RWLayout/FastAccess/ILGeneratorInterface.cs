using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2.FastAccess
{
    public interface IILGenerator
    {

        public ILGenerator Instance { get; }

        //
        // Summary:
        //     Gets the current offset, in bytes, in the Microsoft intermediate language (MSIL)
        //     stream that is being emitted by the System.Reflection.Emit.ILGenerator.
        //
        // Returns:
        //     The offset in the MSIL stream at which the next instruction will be emitted.
        int ILOffset { get; }

        //
        // Summary:
        //     Begins a catch block.
        //
        // Parameters:
        //   exceptionType:
        //     The System.Type object that represents the exception.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The catch block is within a filtered exception.
        //
        //   T:System.ArgumentNullException:
        //     exceptionType is null, and the exception filter block has not returned a value
        //     that indicates that finally blocks should be run until this catch block is located.
        //
        //   T:System.NotSupportedException:
        //     The Microsoft intermediate language (MSIL) being generated is not currently in
        //     an exception block.
        void BeginCatchBlock(Type exceptionType);
        //
        // Summary:
        //     Begins an exception block for a filtered exception.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The Microsoft intermediate language (MSIL) being generated is not currently in
        //     an exception block. -or- This System.Reflection.Emit.ILGenerator belongs to a
        //     System.Reflection.Emit.DynamicMethod.
        void BeginExceptFilterBlock();
        //
        // Summary:
        //     Begins an exception block for a non-filtered exception.
        //
        // Returns:
        //     The label for the end of the block. This will leave you in the correct place
        //     to execute finally blocks or to finish the try.
        Label BeginExceptionBlock();
        //
        // Summary:
        //     Begins an exception fault block in the Microsoft intermediate language (MSIL)
        //     stream.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The MSIL being generated is not currently in an exception block. -or- This System.Reflection.Emit.ILGenerator
        //     belongs to a System.Reflection.Emit.DynamicMethod.
        void BeginFaultBlock();
        //
        // Summary:
        //     Begins a finally block in the Microsoft intermediate language (MSIL) instruction
        //     stream.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The MSIL being generated is not currently in an exception block.
        void BeginFinallyBlock();
        //
        // Summary:
        //     Begins a lexical scope.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     This System.Reflection.Emit.ILGenerator belongs to a System.Reflection.Emit.DynamicMethod.
        void BeginScope();
        //
        // Summary:
        //     Declares a local variable of the specified type, optionally pinning the object
        //     referred to by the variable.
        //
        // Parameters:
        //   localType:
        //     A System.Type object that represents the type of the local variable.
        //
        //   pinned:
        //     true to pin the object in memory; otherwise, false.
        //
        // Returns:
        //     A System.Reflection.Emit.LocalBuilder object that represents the local variable.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     localType is null.
        //
        //   T:System.InvalidOperationException:
        //     The containing type has been created by the System.Reflection.Emit.TypeBuilder.CreateType
        //     method. -or- The method body of the enclosing method has been created by the
        //     System.Reflection.Emit.MethodBuilder.CreateMethodBody(System.Byte[],System.Int32)
        //     method.
        //
        //   T:System.NotSupportedException:
        //     The method with which this System.Reflection.Emit.ILGenerator is associated is
        //     not represented by a System.Reflection.Emit.MethodBuilder.
        LocalBuilder DeclareLocal(Type localType, bool pinned);
        //
        // Summary:
        //     Declares a local variable of the specified type.
        //
        // Parameters:
        //   localType:
        //     A System.Type object that represents the type of the local variable.
        //
        // Returns:
        //     The declared local variable.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     localType is null.
        //
        //   T:System.InvalidOperationException:
        //     The containing type has been created by the System.Reflection.Emit.TypeBuilder.CreateType
        //     method.
        LocalBuilder DeclareLocal(Type localType);
        //
        // Summary:
        //     Declares a new label.
        //
        // Returns:
        //     A new label that can be used as a token for branching.
        Label DefineLabel();
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream followed by the metadata token for the given string.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   str:
        //     The String to be emitted.
        void Emit(OpCode opcode, string str);
        //
        // Summary:
        //     Puts the specified instruction and metadata token for the specified field onto
        //     the Microsoft intermediate language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   field:
        //     A FieldInfo representing a field.
        void Emit(OpCode opcode, FieldInfo field);
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream and leaves space to include a label when fixes are done.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   labels:
        //     The array of label objects to which to branch from this location. All of the
        //     labels will be used.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     con is null. This exception is new in the .NET Framework 4.
        void Emit(OpCode opcode, Label[] labels);
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream and leaves space to include a label when fixes are done.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   label:
        //     The label to which to branch from this location.
        void Emit(OpCode opcode, Label label);
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream followed by the index of the given local variable.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   local:
        //     A local variable.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The parent method of the local parameter does not match the method associated
        //     with this System.Reflection.Emit.ILGenerator.
        //
        //   T:System.ArgumentNullException:
        //     local is null.
        //
        //   T:System.InvalidOperationException:
        //     opcode is a single-byte instruction, and local represents a local variable with
        //     an index greater than Byte.MaxValue.
        void Emit(OpCode opcode, LocalBuilder local);
        //
        // Summary:
        //     Puts the specified instruction and numerical argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   arg:
        //     The Single argument pushed onto the stream immediately after the instruction.
        [SecuritySafeCritical]
        void Emit(OpCode opcode, float arg);
        //
        // Summary:
        //     Puts the specified instruction and character argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   arg:
        //     The character argument pushed onto the stream immediately after the instruction.
        void Emit(OpCode opcode, byte arg);
        //
        // Summary:
        //     Puts the specified instruction and character argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   arg:
        //     The character argument pushed onto the stream immediately after the instruction.
        [CLSCompliant(false)]
        void Emit(OpCode opcode, sbyte arg);
        //
        // Summary:
        //     Puts the specified instruction and numerical argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   arg:
        //     The Int argument pushed onto the stream immediately after the instruction.
        void Emit(OpCode opcode, short arg);
        //
        // Summary:
        //     Puts the specified instruction and numerical argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream. Defined in the OpCodes enumeration.
        //
        //   arg:
        //     The numerical argument pushed onto the stream immediately after the instruction.
        [SecuritySafeCritical]
        void Emit(OpCode opcode, double arg);
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream followed by the metadata token for the given method.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   meth:
        //     A MethodInfo representing a method.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     meth is null.
        //
        //   T:System.NotSupportedException:
        //     meth is a generic method for which the System.Reflection.MethodBase.IsGenericMethodDefinition
        //     property is false.
        [SecuritySafeCritical]
        void Emit(OpCode opcode, MethodInfo meth);
        //
        // Summary:
        //     Puts the specified instruction and numerical argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   arg:
        //     The numerical argument pushed onto the stream immediately after the instruction.
        void Emit(OpCode opcode, int arg);
        //
        // Summary:
        //     Puts the specified instruction onto the stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The Microsoft Intermediate Language (MSIL) instruction to be put onto the stream.
        void Emit(OpCode opcode);
        //
        // Summary:
        //     Puts the specified instruction and numerical argument onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   arg:
        //     The numerical argument pushed onto the stream immediately after the instruction.
        void Emit(OpCode opcode, long arg);
        //
        // Summary:
        //     Puts the specified instruction onto the Microsoft intermediate language (MSIL)
        //     stream followed by the metadata token for the given type.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be put onto the stream.
        //
        //   cls:
        //     A Type.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     cls is null.
        [SecuritySafeCritical]
        void Emit(OpCode opcode, Type cls);
        //
        // Summary:
        //     Puts the specified instruction and a signature token onto the Microsoft intermediate
        //     language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   signature:
        //     A helper for constructing a signature token.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     signature is null.
        void Emit(OpCode opcode, SignatureHelper signature);
        //
        // Summary:
        //     Puts the specified instruction and metadata token for the specified constructor
        //     onto the Microsoft intermediate language (MSIL) stream of instructions.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream.
        //
        //   con:
        //     A ConstructorInfo representing a constructor.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     con is null. This exception is new in the .NET Framework 4.
        [ComVisible(true)]
        [SecuritySafeCritical]
        void Emit(OpCode opcode, ConstructorInfo con);
        //
        // Summary:
        //     Puts a call or callvirt instruction onto the Microsoft intermediate language
        //     (MSIL) stream to call a varargs method.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream. Must be System.Reflection.Emit.OpCodes.Call,
        //     System.Reflection.Emit.OpCodes.Callvirt, or System.Reflection.Emit.OpCodes.Newobj.
        //
        //   methodInfo:
        //     The varargs method to be called.
        //
        //   optionalParameterTypes:
        //     The types of the optional arguments if the method is a varargs method; otherwise,
        //     null.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     opcode does not specify a method call.
        //
        //   T:System.ArgumentNullException:
        //     methodInfo is null.
        //
        //   T:System.InvalidOperationException:
        //     The calling convention for the method is not varargs, but optional parameter
        //     types are supplied. This exception is thrown in the .NET Framework versions 1.0
        //     and 1.1, In subsequent versions, no exception is thrown.
        [SecuritySafeCritical]
        void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes);
        //
        // Summary:
        //     Puts a System.Reflection.Emit.OpCodes.Calli instruction onto the Microsoft intermediate
        //     language (MSIL) stream, specifying a managed calling convention for the indirect
        //     call.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream. Must be System.Reflection.Emit.OpCodes.Calli.
        //
        //   callingConvention:
        //     The managed calling convention to be used.
        //
        //   returnType:
        //     The System.Type of the result.
        //
        //   parameterTypes:
        //     The types of the required arguments to the instruction.
        //
        //   optionalParameterTypes:
        //     The types of the optional arguments for varargs calls.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     optionalParameterTypes is not null, but callingConvention does not include the
        //     System.Reflection.CallingConventions.VarArgs flag.
        [SecuritySafeCritical]
        void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes);
        //
        // Summary:
        //     Puts a System.Reflection.Emit.OpCodes.Calli instruction onto the Microsoft intermediate
        //     language (MSIL) stream, specifying an unmanaged calling convention for the indirect
        //     call.
        //
        // Parameters:
        //   opcode:
        //     The MSIL instruction to be emitted onto the stream. Must be System.Reflection.Emit.OpCodes.Calli.
        //
        //   unmanagedCallConv:
        //     The unmanaged calling convention to be used.
        //
        //   returnType:
        //     The System.Type of the result.
        //
        //   parameterTypes:
        //     The types of the required arguments to the instruction.
        //void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes);

        //
        // Summary:
        //     Emits the Microsoft intermediate language (MSIL) to call Overload:System.Console.WriteLine
        //     with a string.
        //
        // Parameters:
        //   value:
        //     The string to be printed.
        void EmitWriteLine(string value);
        //
        // Summary:
        //     Emits the Microsoft intermediate language (MSIL) necessary to call Overload:System.Console.WriteLine
        //     with the given field.
        //
        // Parameters:
        //   fld:
        //     The field whose value is to be written to the console.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     There is no overload of the Overload:System.Console.WriteLine method that accepts
        //     the type of the specified field.
        //
        //   T:System.ArgumentNullException:
        //     fld is null.
        //
        //   T:System.NotSupportedException:
        //     The type of the field is System.Reflection.Emit.TypeBuilder or System.Reflection.Emit.EnumBuilder,
        //     which are not supported.
        void EmitWriteLine(FieldInfo fld);
        //
        // Summary:
        //     Emits the Microsoft intermediate language (MSIL) necessary to call Overload:System.Console.WriteLine
        //     with the given local variable.
        //
        // Parameters:
        //   localBuilder:
        //     The local variable whose value is to be written to the console.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The type of localBuilder is System.Reflection.Emit.TypeBuilder or System.Reflection.Emit.EnumBuilder,
        //     which are not supported. -or- There is no overload of Overload:System.Console.WriteLine
        //     that accepts the type of localBuilder.
        //
        //   T:System.ArgumentNullException:
        //     localBuilder is null.
        void EmitWriteLine(LocalBuilder localBuilder);
        //
        // Summary:
        //     Ends an exception block.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The end exception block occurs in an unexpected place in the code stream.
        //
        //   T:System.NotSupportedException:
        //     The Microsoft intermediate language (MSIL) being generated is not currently in
        //     an exception block.
        void EndExceptionBlock();
        //
        // Summary:
        //     Ends a lexical scope.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     This System.Reflection.Emit.ILGenerator belongs to a System.Reflection.Emit.DynamicMethod.
        void EndScope();
        //
        // Summary:
        //     Marks the Microsoft intermediate language (MSIL) stream's current position with
        //     the given label.
        //
        // Parameters:
        //   loc:
        //     The label for which to set an index.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     loc represents an invalid index into the label array. -or- An index for loc has
        //     already been defined.
        void MarkLabel(Label loc);
        //
        // Summary:
        //     Emits an instruction to throw an exception.
        //
        // Parameters:
        //   excType:
        //     The class of the type of exception to throw.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     excType is not the System.Exception class or a derived class of System.Exception.
        //     -or- The type does not have a default constructor.
        //
        //   T:System.ArgumentNullException:
        //     excType is null.
        void ThrowException(Type excType);
        //
        // Summary:
        //     Specifies the namespace to be used in evaluating locals and watches for the current
        //     active lexical scope.
        //
        // Parameters:
        //   usingNamespace:
        //     The namespace to be used in evaluating locals and watches for the current active
        //     lexical scope
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Length of usingNamespace is zero.
        //
        //   T:System.ArgumentNullException:
        //     usingNamespace is null.
        //
        //   T:System.NotSupportedException:
        //     This System.Reflection.Emit.ILGenerator belongs to a System.Reflection.Emit.DynamicMethod.
        void UsingNamespace(string usingNamespace);
    }

    class ILGenerator_ : IILGenerator
    {
        public ILGenerator Instance { get; private set; }
        public ILGenerator_(ILGenerator instance)
        {
            this.Instance = instance;
        }

        public int ILOffset => Instance.ILOffset;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginCatchBlock(Type exceptionType) => Instance.BeginCatchBlock(exceptionType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginExceptFilterBlock() => Instance.BeginExceptFilterBlock();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Label BeginExceptionBlock() => Instance.BeginExceptionBlock();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFaultBlock() => Instance.BeginFaultBlock();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFinallyBlock() => Instance.BeginFinallyBlock();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginScope() => Instance.BeginScope();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalBuilder DeclareLocal(Type localType, bool pinned) => Instance.DeclareLocal(localType, pinned);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LocalBuilder DeclareLocal(Type localType) => Instance.DeclareLocal(localType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Label DefineLabel() => Instance.DefineLabel();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, string str) => Instance.Emit(opcode, str);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, FieldInfo field) => Instance.Emit(opcode, field);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, Label[] labels) => Instance.Emit(opcode, labels);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, Label label) => Instance.Emit(opcode, label);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, LocalBuilder local) => Instance.Emit(opcode, local);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, float arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, byte arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, sbyte arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, short arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, double arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, MethodInfo meth) => Instance.Emit(opcode, meth);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, int arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode) => Instance.Emit(opcode);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, long arg) => Instance.Emit(opcode, arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, Type cls) => Instance.Emit(opcode, cls);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, SignatureHelper signature) => Instance.Emit(opcode, signature);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Emit(OpCode opcode, ConstructorInfo con) => Instance.Emit(opcode, con);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) => Instance.EmitCall(opcode, methodInfo, optionalParameterTypes);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) => Instance.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes) => Instance.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EmitWriteLine(string value) => Instance.EmitWriteLine(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EmitWriteLine(FieldInfo fld) => Instance.EmitWriteLine(fld);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EmitWriteLine(LocalBuilder localBuilder) => Instance.EmitWriteLine(localBuilder);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndExceptionBlock() => Instance.EndExceptionBlock();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndScope() => Instance.EndScope();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MarkLabel(Label loc) => Instance.MarkLabel(loc);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowException(Type excType) => Instance.ThrowException(excType);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UsingNamespace(string usingNamespace) => Instance.UsingNamespace(usingNamespace);
    }

    static class ILGeneratorInterfaceExt
    {
        public static IILGenerator AsInterface(this ILGenerator instance) => new LoggingILGenerator(instance);
    }

    class LoggingILGenerator : IILGenerator
    {
        public ILGenerator Instance { get; private set; }
        public LoggingILGenerator(ILGenerator instance)
        {
            this.Instance = instance;
        }

        public int ILOffset => Instance.ILOffset;

        public void BeginCatchBlock(Type exceptionType) => Instance.BeginCatchBlock(exceptionType);
        public void BeginExceptFilterBlock() => Instance.BeginExceptFilterBlock();
        public Label BeginExceptionBlock() => Instance.BeginExceptionBlock();
        public void BeginFaultBlock() => Instance.BeginFaultBlock();
        public void BeginFinallyBlock() => Instance.BeginFinallyBlock();
        public void BeginScope() => Instance.BeginScope();
        public LocalBuilder DeclareLocal(Type localType, bool pinned) => Instance.DeclareLocal(localType, pinned);
        public LocalBuilder DeclareLocal(Type localType) => Instance.DeclareLocal(localType);
        public Label DefineLabel() => Instance.DefineLabel();

        public void Emit(OpCode opcode, string str) { Debug.WriteLine($"emit: {opcode} {str}"); Instance.Emit(opcode, str); }
        public void Emit(OpCode opcode, FieldInfo field) { Debug.WriteLine($"emit: {opcode} {field}"); Instance.Emit(opcode, field); }
        public void Emit(OpCode opcode, Label[] labels) { Debug.WriteLine($"emit: {opcode} {string.Join(";", labels)}"); Instance.Emit(opcode, labels); }
        public void Emit(OpCode opcode, Label label) { Debug.WriteLine($"emit: {opcode} {label}"); Instance.Emit(opcode, label); }
        public void Emit(OpCode opcode, LocalBuilder local) { Debug.WriteLine($"emit: {opcode} {local}"); Instance.Emit(opcode, local); }
        public void Emit(OpCode opcode, float arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, byte arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, sbyte arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, short arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, double arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, MethodInfo meth) { Debug.WriteLine($"emit: {opcode} {meth}"); Instance.Emit(opcode, meth); }
        public void Emit(OpCode opcode, int arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode) { Debug.WriteLine($"emit: {opcode}"); Instance.Emit(opcode); }
        public void Emit(OpCode opcode, long arg) { Debug.WriteLine($"emit: {opcode} {arg}"); Instance.Emit(opcode, arg); }
        public void Emit(OpCode opcode, Type cls) { Debug.WriteLine($"emit: {opcode} {cls}"); Instance.Emit(opcode, cls); }
        public void Emit(OpCode opcode, SignatureHelper signature) { Debug.WriteLine($"emit: {opcode} {signature}"); Instance.Emit(opcode, signature); }
        public void Emit(OpCode opcode, ConstructorInfo con) { Debug.WriteLine($"emit: {opcode} {con}"); Instance.Emit(opcode, con); }
        public void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) { Debug.WriteLine($"emit call: {opcode} {methodInfo} {string.Join(";", (IEnumerable<Type>)optionalParameterTypes)}"); Instance.EmitCall(opcode, methodInfo, optionalParameterTypes); }
        public void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) { Debug.WriteLine($"emit call i: {opcode} {callingConvention} {returnType} {string.Join(";", (IEnumerable<Type>)parameterTypes)} {string.Join(";", (IEnumerable<Type>)optionalParameterTypes)}"); Instance.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes); }
        //public void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes) { Debug.WriteLine($"emit call i: {opcode} {unmanagedCallConv} {returnType} {string.Join(";", (IEnumerable<Type>)parameterTypes)}"); Instance.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes); }
        public void EmitWriteLine(string value) { Debug.WriteLine($"emit writeline: {value}"); Instance.EmitWriteLine(value); }
        public void EmitWriteLine(FieldInfo fld) { Debug.WriteLine($"emit writeline: {fld}"); Instance.EmitWriteLine(fld); }
        public void EmitWriteLine(LocalBuilder localBuilder) { Debug.WriteLine($"emit writeline: {localBuilder}"); Instance.EmitWriteLine(localBuilder); }


        public void EndExceptionBlock() => Instance.EndExceptionBlock();
        public void EndScope() => Instance.EndScope();
        public void MarkLabel(Label loc) => Instance.MarkLabel(loc);
        public void ThrowException(Type excType) => Instance.ThrowException(excType);
        public void UsingNamespace(string usingNamespace) => Instance.UsingNamespace(usingNamespace);    }
}
