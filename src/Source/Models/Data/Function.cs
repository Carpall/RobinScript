using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RobinVM.Models
{
    public struct Function
    {
        public static Function New(int paramCount, params Instruction[] instructions) => new Function(instructions, paramCount);
        public Function(Instruction[] instructions, int paramCount)
        {
            Instructions = instructions;
            _arguments = null;
            _labels = new Dictionary<string, int>();
            _paramCount = paramCount;
        }
        public void AddLabel(string label, int instructionIndex)
        {
            if (!_labels.TryAdd(label, instructionIndex))
                BasePanic.Throw($"Already define label `{label}` as {_labels[label]}: `{Instructions[_labels[label]].FunctionPointer.Method.Name}`", 2, "PreRuntime");
        }
        public int FindLabel(string label)
        {
            if (_labels.TryGetValue(label, out int ret))
                return ret;
            BasePanic.Throw($"Undefine label `{label}`", 3, "Runtime");
            return 0;
        }
        public bool UninstantiatedLabels()
        {
            return _labels == null;
        }
        public object FindArgument(byte index)
        {
            if (_arguments is null)
                BasePanic.Throw($"Insufficient function arguments, have not been passed {index+1} argument/s", 4, "Runtime");
            if (index < 0)
                BasePanic.Throw("Can not index function argument with a negative index", 5, "Runtime");
            if (index < _arguments.Count)
                return _arguments[index];
            BasePanic.Throw($"Insufficient function arguments, have not been passed {index+1} argument/s", 6, "Runtime");
            return null;
        }
        public void PassArguments(List<object> arguments)
        {
            _arguments = arguments;
        }
        public int ParamCount()
        {
            return _paramCount;
        }
        int _paramCount;
        Dictionary<string, int> _labels;
        List<object> _arguments;
        public Instruction[] Instructions;
    }
}