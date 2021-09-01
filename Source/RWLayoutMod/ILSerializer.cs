using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutMod
{

    static class ILSerializer
    {
        internal static string SerializeInstructions(IEnumerable<CodeInstruction> instuctions)
        {
            LabelIndexes.Clear();

            var sb = new StringBuilder();
            using (var ildesc = new StringWriter(sb))
            {
                foreach (var instr in instuctions)
                {
                    WriteOpcode(ildesc, instr.opcode);
                    WriteOperand(ildesc, instr.operand);
                    WriteLabels(ildesc, instr.labels);
                    WriteBlocks(ildesc, instr.blocks);
                }
            }

            return sb.ToString();
        }

        internal static string LogSInstructions(IEnumerable<CodeInstruction> instuctions)
        {
            LabelIndexes.Clear();

            var sb = new StringBuilder();
            using (var ildesc = new System.IO.StringWriter(sb))
            {
                foreach (var instr in instuctions)
                {
                    ildesc.WriteLine(instr.ToString());
                }
            }

            return sb.ToString();
        }

        private enum OperandType
        {
            Unknown,
            String,
            ConstructorInfo,
            MethodInfo,
            FieldInfo,
            Label,
            Local,
            Type,
        }

        private static void WriteOpcode(TextWriter node, OpCode opcode)
        {
            node.WriteLine($"opcode: {opcode}");
        }

        private static void WriteOperand(TextWriter node, object operand)
        {
            if (operand == null)
            {
                return;
            }

            if (operand is String)
            {
                WriteOperandType(node, OperandType.String);
                node.WriteLine((String)operand);
            }
            else if (operand is Type)
            {
                WriteOperandType(node, OperandType.Type);
                node.WriteLine(((Type)operand).FullName);
            }

            else if (operand is ConstructorInfo)
            {
                WriteOperandType(node, OperandType.ConstructorInfo);
                var ctor = (ConstructorInfo)operand;
                WriteMethod(node, ctor);
            }
            else if (operand is MethodInfo)
            {
                WriteOperandType(node, OperandType.MethodInfo);
                var method = (MethodInfo)operand;
                WriteMethod(node, method);
            }
            else if (operand is FieldInfo)
            {
                WriteOperandType(node, OperandType.FieldInfo);
                var field = (FieldInfo)operand;
                WriteField(node, field);
            }

            else if (operand is Label)
            {
                WriteOperandType(node, OperandType.Label);
                node.WriteLine(GetLabelName((Label)operand));
            }
            else if (operand is LocalBuilder)
            {
                WriteOperandType(node, OperandType.Local);
                WriteLocal(node, (LocalBuilder)operand);
            }
            else
            {
                node.WriteLine($"unknown operand type: {operand.GetType().FullName}");
            }
        }

        private static void WriteMethod(TextWriter operandNode, MethodBase method)
        {
            operandNode.WriteLine(method.FullDescription());
        }

        private static void WriteField(TextWriter operandNode, FieldInfo field)
        {
            operandNode.WriteLine(field.ToString());
        }


        private static void WriteLocal(TextWriter operandNode, LocalBuilder local)
        {
            operandNode.WriteLine($"{local.LocalIndex}/{local.LocalType.FullName}/{local.IsPinned}");
        }

        private static void WriteOperandType(TextWriter operandNode, OperandType type)
        {
            operandNode.WriteLine($"type: {type}");
        }

        private static void WriteLabels(TextWriter node, List<Label> labels)
        {
            if (labels.Count == 0)
            {
                return;
            }
            node.WriteLine($"labels: {String.Join("|", labels.Select(x => GetLabelName(x)))}");
        }

        private static void WriteBlocks(StringWriter node, List<ExceptionBlock> blocks)
        {
            if (blocks.Count == 0)
            {
                return;
            }
            node.WriteLine("blocks:");
            foreach (var block in blocks)
            {
                node.WriteLine($"{block.blockType};{block.catchType?.FullName ?? ""}");
            }
        }

        private static Dictionary<Label, int> LabelIndexes = new Dictionary<Label, int>();
        private static string GetLabelName(Label label)
        {
            int index;
            if (!LabelIndexes.TryGetValue(label, out index))
            {
                index = LabelIndexes.Count;
                LabelIndexes.Add(label, index);
            }

            return $"lbl{index}";
        }

        private static string ExceptionBlockToString(ExceptionBlock block)
        {
            return $"{block.blockType}|{block.catchType.AssemblyQualifiedName}";
        }
    }

}
