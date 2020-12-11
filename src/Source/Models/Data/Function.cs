using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RobinVM.Models
{
    public struct Function
    {
        public static Function New(params Instruction[] instructions) => new Function(instructions);
        public Function(Instruction[] instructions)
        {
            Instructions = instructions;
            Arguments = null;
            Labels = new Dictionary<string, int>();
        }
        public void AddLabel(string label, int instructionIndex)
        {
            if (!Labels.TryAdd(label, instructionIndex))
                BasePanic.Throw($"Already define label `{label}` as {Labels[label]}: `{Instructions[Labels[label]].FunctionPointer.Method.Name}`", 2, "PreRuntime");
        }
        public int FindLabel(string label)
        {
            if (Labels.TryGetValue(label, out int ret))
                return ret;
            BasePanic.Throw($"Undefine label `{label}`", 3, "Runtime");
            return 0;
        }
        public bool UninstantiatedLabels()
        {
            return Labels == null;
        }
        public object FindArgument(byte index)
        {
            if (Arguments is null)
                BasePanic.Throw($"Insufficient function arguments, have not been passed {index+1} argument/s", 4, "Runtime");
            if (index < 0)
                BasePanic.Throw("Can not index function argument with a negative index", 5, "Runtime");
            if (index < Arguments.Length)
                return Arguments[index];
            BasePanic.Throw($"Insufficient function arguments, have not been passed {index+1} argument/s", 6, "Runtime");
            return null;
        }
        public void PassArguments(object[] arguments)
        {
            Arguments = arguments;
        }
        public void LoadArgumentsAsList()
        {
            Runtime.Stack.Push(Arguments[1..^0].ToList());
        }
        Dictionary<string, int> Labels;
        object[] Arguments;
        public Instruction[] Instructions;
    }
}