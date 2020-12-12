using System;
using System.Collections.Generic;

namespace RobinVM.Models
{
    public static class BasePanic
    {
        public static int? TryScopeTarget = null;
        static List<string> Trace = new List<string>();
        public static T Cast<T>(this object self) {
            if (self is not T)
            {
                var inner = Runtime.RuntimeImage.FindObj("basepanic").Copy().CacheTable;
                inner["msg"] = $"Tryed to cast {self.GetType()} to {typeof(T)}";
                inner["code"] = 41;
                inner["type"] = "CastException";
                BasePanic.Throw(inner);
            }
            return (T)self;
        }
        public static void Throw(string error, int code, string state)
        {
            Console.WriteLine("BasePanic[{2}: {3}]: {0}\nTrace:\n   at: {1}", error, Trace.Count == 0 ? "$(No Trace)" : string.Join("\n   in: ", Trace), state, code);
            Environment.Exit(code);
        }
        public static void Throw(CacheTable inner)
        {
            if (TryScopeTarget != null)
            {
                Runtime.ProgramCounter = (int)TryScopeTarget - 1;
                Runtime.Stack.Push(inner);
                return;
            }
            Console.WriteLine("BasePanic[{2}: {3}]: {0}\nTrace:\n   at: {1}", inner["msg"], Trace.Count == 0 ? "$(No Trace)" : string.Join("\n   in: ", Trace), inner["type"], inner["code"]);
            Environment.Exit((int)inner["code"]);
        }
        public static void LoadTrail(string trail)
        {
            BasePanic.Trace.Insert(0, trail);
        }
        public static void UnloadTrail()
        {
            BasePanic.Trace.RemoveAt(0);
        }
    }
}
