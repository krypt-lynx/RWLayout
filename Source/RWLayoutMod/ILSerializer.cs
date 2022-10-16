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

        static Dictionary<Type, OperandType> OpTypeForType = new Dictionary<Type, OperandType>
        {
            {  typeof(string), OperandType.String },
            {  typeof(Type), OperandType.Type },
            {  typeof(ConstructorInfo), OperandType.ConstructorInfo },
            {  typeof(MethodInfo), OperandType.MethodInfo },
            {  typeof(FieldInfo), OperandType.FieldInfo },
            {  typeof(Label), OperandType.Label },
            {  typeof(LocalBuilder), OperandType.Local },
        };

        static Dictionary<OperandType, Action<TextWriter, object>> writerForOpType = new Dictionary<OperandType, Action<TextWriter, object>>
        {
            { OperandType.String, (n, o) => { n.WriteLine(o); } },
            { OperandType.Type, (n, o) => { n.WriteLine(((Type)o).FullName); } },
            { OperandType.ConstructorInfo, (n, o) => { WriteMethod(n, (ConstructorInfo)o); } },
            { OperandType.MethodInfo, (n, o) => { WriteMethod(n, (MethodInfo)o); } },
            { OperandType.FieldInfo, (n, o) => { WriteField(n, (FieldInfo)o); } },
            { OperandType.Label, (n, o) => { n.WriteLine(GetLabelName((Label)o)); } },
            { OperandType.Local, (n, o) => { WriteLocal(n, (LocalBuilder)o); } },
        };

        private static void WriteOperand(TextWriter node, object operand)
        {
            if (operand == null)
            {
                return;
            }

            OperandType opType = GetOpType(operand);
            if (opType != OperandType.Unknown)
            {
                WriteOperandType(node, opType);
                writerForOpType[opType](node, operand);
            }
            else
            {
                node.WriteLine($"unknown operand type: {operand.GetType().FullName}");
            }
        }

        private static OperandType GetOpType(object operand)
        {
            OperandType opType = OperandType.Unknown;
            var lookupType = operand.GetType();
            while (lookupType != null)
            {
                if (OpTypeForType.TryGetValue(lookupType, out opType))
                {
                    break;
                }
                lookupType = lookupType.BaseType;
            }

            return opType;
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
