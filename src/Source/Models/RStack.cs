using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RobinVM.Models
{
    public struct RStack
    {
        public RStack(int maxStack)
        {
            VirtualStack = new List<object>(maxStack);
        }
        List<object> VirtualStack;
        public int CountStack()
        {
            return VirtualStack.Count;
        }
        public List<object> PopRangeAsList(int size)
        {
            var x = VirtualStack.GetRange(VirtualStack.Count - size, size);
            VirtualStack.RemoveRange(VirtualStack.Count - size, size);
            return x;
        }
        public void TransferToArguments(ref Function function)
        {
            var cs = CountStack();
            var index = cs - function.ParamCount();
            var count = cs-index;
            var args = VirtualStack.GetRange(index, count);
            VirtualStack.RemoveRange(index, count);
            function.PassArguments(args);
        }
        public object Pop()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 13, "Runtime");
            var x0 = VirtualStack[^1];
            VirtualStack.RemoveAt(VirtualStack.Count - 1);
            return x0;
        }
        public T Pop<T>()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 12, "Runtime");
            var x0 = VirtualStack[^1];
            VirtualStack.RemoveAt(VirtualStack.Count - 1);
            return x0.Cast<T>();
        }
        public T Peek<T>(byte skip)
        {
            if (VirtualStack.Count < skip)
                BasePanic.Throw("Empty stack", 12, "Runtime");
            var x0 = VirtualStack[^skip];
            return x0.Cast<T>();
        }
        public T PrePop<T>()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 12, "Runtime");
            var x0 = VirtualStack[0];
            VirtualStack.RemoveAt(0);
            return x0.Cast<T>();
        }
        public object Peek()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 17, "Runtime");
            return VirtualStack[^1];
        }
        public T PrePeek<T>()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 11, "Runtime");
            return VirtualStack[0].Cast<T>();
        }
        public void Push(object value)
        {
            if (VirtualStack.Count == 16)
                BasePanic.Throw("Empty stack", 10, "Runtime");
            VirtualStack.Add(value);
        }
        public void PrePush(object value)
        {
            if (VirtualStack.Count == 16)
                BasePanic.Throw("Empty stack", 9, "Runtime");
            VirtualStack.Insert(0, value);
        }
        public void PopNR()
        {
            if (VirtualStack.Count == 0)
                BasePanic.Throw("Empty stack", 8, "Runtime");
            VirtualStack.RemoveAt(VirtualStack.Count - 1);
        }
        public void Clear()
        {
            if (VirtualStack.Count != 0)
            VirtualStack.Clear();
        }
        public void DrawStack(object nill = null)
        {
            Console.WriteLine("Stack Count: {0}/{1}\nStack Draw:", VirtualStack.Count, VirtualStack.Capacity);
            if (VirtualStack.Count == 0)
                Console.WriteLine("   Empty Stack");
            for (int i = VirtualStack.Count - 1; i >= 0; i--)
                Console.WriteLine("   {0} | Object[{2}]: `{1}`", i, VirtualStack[i], VirtualStack[i].GetType().ToString());
        }
    }
}
