using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutTests.FastAccess
{
    public class TestIlGenerator : IILGenerator
    {
        public OpCode? lastOpCode { get; private set; } = null;
        public object lastOperand { get; private set; } = null;
        public void ResetState()
        {
            lastOpCode = null;
            lastOperand = null;
        }


        public ILGenerator Instance => throw new NotImplementedException();

        public int ILOffset => throw new NotImplementedException();

        public void BeginCatchBlock(Type exceptionType)
        {
            throw new NotImplementedException();
        }

        public void BeginExceptFilterBlock()
        {
            throw new NotImplementedException();
        }

        public Label BeginExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public void BeginFaultBlock()
        {
            throw new NotImplementedException();
        }

        public void BeginFinallyBlock()
        {
            throw new NotImplementedException();
        }

        public void BeginScope()
        {
            throw new NotImplementedException();
        }

        public LocalBuilder DeclareLocal(Type localType, bool pinned)
        {
            throw new NotImplementedException();
        }

        public LocalBuilder DeclareLocal(Type localType)
        {
            throw new NotImplementedException();
        }

        public Label DefineLabel()
        {
            throw new NotImplementedException();
        }

        public void Emit(OpCode opcode, string str)
        {
            lastOpCode = opcode;
            lastOperand = str;
        }

        public void Emit(OpCode opcode, FieldInfo field)
        {
            lastOpCode = opcode;
            lastOperand = field;
        }

        public void Emit(OpCode opcode, Label[] labels)
        {
            lastOpCode = opcode;
            lastOperand = labels;
        }

        public void Emit(OpCode opcode, Label label)
        {
            lastOpCode = opcode;
            lastOperand = label;
        }

        public void Emit(OpCode opcode, LocalBuilder local)
        {
            lastOpCode = opcode;
            lastOperand = local;
        }

        public void Emit(OpCode opcode, float arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, byte arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, sbyte arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, short arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, double arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, MethodInfo meth)
        {
            lastOpCode = opcode;
            lastOperand = meth;
        }

        public void Emit(OpCode opcode, int arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode)
        {
            lastOpCode = opcode;
            lastOperand = null;
        }

        public void Emit(OpCode opcode, long arg)
        {
            lastOpCode = opcode;
            lastOperand = arg;
        }

        public void Emit(OpCode opcode, Type cls)
        {
            lastOpCode = opcode;
            lastOperand = cls;
        }

        public void Emit(OpCode opcode, SignatureHelper signature)
        {
            lastOpCode = opcode;
            lastOperand = signature;
        }

        public void Emit(OpCode opcode, ConstructorInfo con)
        {
            lastOpCode = opcode;
            lastOperand = con;
        }

        public void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            throw new NotImplementedException();
        }

        public void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
        {
            throw new NotImplementedException();
        }

        public void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        public void EmitWriteLine(string value)
        {
            throw new NotImplementedException();
        }

        public void EmitWriteLine(FieldInfo fld)
        {
            throw new NotImplementedException();
        }

        public void EmitWriteLine(LocalBuilder localBuilder)
        {
            throw new NotImplementedException();
        }

        public void EndExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public void EndScope()
        {
            throw new NotImplementedException();
        }

        public void MarkLabel(Label loc)
        {
            throw new NotImplementedException();
        }

        public void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
        {
            throw new NotImplementedException();
        }

        public void ThrowException(Type excType)
        {
            throw new NotImplementedException();
        }

        public void UsingNamespace(string usingNamespace)
        {
            throw new NotImplementedException();
        }
    }


    [TestClass]
    public class IILGenerator_Emit
    {
        [TestMethod]
        public void MatchingOpCodeEmited()
        {
            //var allOpcodes = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.FieldType == typeof(OpCode)).Select(x => x.Name).ToHashSet();
            var gen = new TestIlGenerator();

            var methods = typeof(ILGeneratorEmit).GetMethods(BindingFlags.Public | BindingFlags.Static);

            Assert.AreNotEqual(methods.Length, 0, "No OpCode shortcut methods found");

            foreach (var method in methods)
            {
                var args = new object[method.GetParameters().Length];
                args[0] = gen;
                for (int i = 1, imax = method.GetParameters().Length; i < imax; i++)
                {
                    args[i] = null;
                }

                var ret = method.Invoke(null, args);
                Assert.IsNotNull(gen, $"method {method.Name} returned null");
                Assert.AreEqual(gen, ret, $"different generator was returned by method {method.Name}");
                Assert.AreEqual(method.Name.ToLowerInvariant(), gen.lastOpCode?.Name.ToLowerInvariant().Replace('.', '_'), $"Method {method.Name} used {gen.lastOpCode?.Name} opcode instead");
            }



        }
    }
}
