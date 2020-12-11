using RobinVM.Models;
using System;
using System.Diagnostics;
using CacheTable = System.Collections.Generic.Dictionary<string, object>;
namespace RobinVM
{
    public static class Runtime
    {
        public delegate void RuntimePointer(object args);
        public delegate void CallPointer();
        public static object[] Storage = new object[byte.MaxValue];
        public static byte StorageManager = 0;
        public static int ProgramCounter = 0;
        public static Image RuntimeImage;
        public static Function CurrentFunctionPointer;
        public static readonly RStack Stack = new RStack(600000);

        /// <summary>
        /// Matches if stack peek type is the same of <typeparamref name="T"/>
        /// </summary>
        /// <param name="args"></param>
        public static void MatchType<T>(object args) => Stack.Push(Stack.Peek() is T);

        /// <summary>
        /// Casts last element onto the stack to int32 and pushes result
        /// </summary>
        /// <param name="args"></param>
        public static void CastToInt(object args) => Stack.Push(Convert.ToInt32(Stack.Pop()));

        /// <summary>
        /// Casts last element onto the stack to <typeparamref name="T"/> and pushes result
        /// </summary>
        /// <param name="args"></param>
        public static void Cast<T>(object args) => Stack.Push(Convert.ChangeType(Stack.Pop(), typeof(T)));

        /// <summary>
        /// Casts last element onto the stack to float and push result
        /// </summary>
        /// <param name="args"></param>
        public static void CastToFloat(object args) => Stack.Push(Convert.ToSingle(Stack.Pop()));

        /// <summary>
        /// Casts last element onto the stack to bool and push result
        /// </summary>
        /// <param name="args"></param>
        public static void CastToBool(object args) => Stack.Push(Convert.ToBoolean(Stack.Pop()));

        /// <summary>
        /// Casts last element onto the stack to string and push result
        /// </summary>
        /// <param name="args"></param>
        public static void CastToString(object args) => Stack.Push(Stack.Pop().ToString());

        /// <summary>
        /// Stores the value onto the stack in the local heap
        /// </summary>
        /// <param name="args">Index of the slot into store last stack element</param>
        public static void Store(object args)
        {
            Storage[Convert.ToByte(args)] = Stack.Pop();
            StorageManager++;
        }

        /// <summary>
        /// Frees a local heap slot
        /// </summary>
        /// <param name="args">Index of the slot to free</param>
        public static void Restore(object args)
        {
            Storage[Convert.ToByte(args)] = null;
            StorageManager--;
        }

        /// <summary>
        /// Pops the instance loaded onto the stack and store the global
        /// </summary>
        /// <param name="args">Name of global</param>
        public static void StoreGlobal(object args)
        {
            var p = Stack.Pop();
            Stack.Pop<CacheTable>()[(string)args] = p;
        }

        /// <summary>
        /// Calls a function
        /// </summary>
        /// <param name="args">Name of function</param>
        public static void Call(object args) => RuntimeImage.FindFunction((string)args).ExecuteLabel((string)args);

        /// <summary>
        /// Pops the instance loaded onto the stack and calls the function
        /// </summary>
        /// <param name="args">Index of function to call</param>
        public static void CallInstance(object args)
        {
            var p = Stack.PrePeek<CacheTable>();
            ((p[(string)args]).Cast<Function>()).ExecuteLabel("ins "+p["$"]+":" + (string)args);
        }

        /// <summary>
        /// Loads onto the stack a new instance of the obj and call its ctor
        /// </summary>
        /// <param name="args">Name of obj</param>
        public static void NewObj(object args)
        {
            var ins = RuntimeImage.FindObj((string)args).Copy();
            Stack.PrePush(ins.CacheTable);
            ins.Ctor.Value.ExecuteLabel("ins "+(string)args+":ctor");
            Stack.Push(ins.CacheTable);
        }
        /// <summary>
        /// Loads onto the stack a constant
        /// </summary>
        /// <param name="args">Constant to load onto the stack</param>
        public static void Load(object args) => Stack.Push(args);

        /// <summary>
        /// Loads onto the stack a global variable
        /// </summary>
        /// <param name="args">Global variable id</param>
        public static void LoadGlobal(object args) => Stack.Push(Stack.Pop<CacheTable>()[(string)args]);

        /// <summary>
        /// Loads onto the stack a function argument
        /// </summary>
        /// <param name="args">Constant to load onto the stack</param>
        public static void LoadFromArgs(object args) => Stack.Push(CurrentFunctionPointer.FindArgument(Convert.ToByte(args)));

        /// <summary>
        /// Loads onto the stack the current runtime image
        /// </summary>
        /// <param name="args">G-lobal variable id</param>
        public static void LoadRuntimeImage(object args) => Stack.Push(RuntimeImage.GetCacheTable());

        /// <summary>
        /// Takes last element on the stack as index, the second last as the new value to replace with and the tird last
        /// as array to operate in
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T">Type of array<br/>Example: int[] => StoreElementIntoArray&lt;int&gt;()</typeparam>
        public static void StoreElementIntoArray(object args)
        {
            var p = Stack.Pop<int>();
            var val = Stack.Pop();
            var arr = Stack.Pop<object[]>();
            arr[p] = val;
            Stack.Push(arr);
        }

        /// <summary>
        /// Takes last element on the stack as index of the element to push onto the stack, the second last as array
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T">Type of array<br/>Example: int[] => StoreElementIntoArray&lt;int&gt;()</typeparam>
        public static void LoadElementFromArray(object args)
        {
            var p = Stack.Pop<int>();
            Stack.Push((Stack.Pop<object[]>())[p]);
        }

        /// <summary>
        /// Clears stack
        /// </summary>
        /// <param name="args"></param>
        public static void Clear(object args) => Stack.Clear();

        /// <summary>
        /// Loads from local heap onto the stack
        /// </summary>
        /// <param name="args">Index of local heap to load</param>
        public static void LoadFromStorage(object args) => Stack.Push(Storage[Convert.ToByte(args)]);

        /// <summary>
        /// Initialize try-panic environment
        /// </summary>
        /// <param name="args">Instruction index of relative OnPanic statement</param>
        public static void Try(object args)
        {
            if (BasePanic.TryScopeTarget != null)
                BasePanic.Throw("Can not initialize try environment in the scope of another try environment", 14, "PreRuntime");
            BasePanic.TryScopeTarget = args;
        }
        /// <summary>
        /// Initialize try-panic environment
        /// </summary>
        /// <param name="args">Label name of relative OnPanic statement</param>
        public static void TryLabel(object args)
        {
            if (BasePanic.TryScopeTarget != null)
                BasePanic.Throw("Can not initialize try environment in the scope of another try environment", 7, "PreRuntime");
            BasePanic.TryScopeTarget = CurrentFunctionPointer.FindLabel((string)args);
        }

        /// <summary>
        /// Closes try-panic environment
        /// </summary>
        /// <param name="args"></param>
        public static void Finally(object args) => BasePanic.TryScopeTarget = null;

        /// <summary>
        /// If try scope has not paniced, the program will jump to <paramref name="args"/>
        /// </summary>
        /// <param name="args"></param>
        public static void OnPanic(object args) => BasePanic.TryScopeTarget = null;

        /// <summary>
        /// Adds last element with second last and pushes it onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Add(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Tryed to perform operation `Add` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 16, "Runtime");
            if (p is string) Stack.Push((string)p1 + (string)p);
            else if (p is byte) Stack.Push((byte)p1 + (byte)p);
            else if (p is short) Stack.Push((short)p1 + (short)p);
            else if (p is int) Stack.Push((int)p1 + (int)p);
            else if (p is long) Stack.Push((long)p1 + (long)p);
            else if (p is float) Stack.Push((float)p1 + (float)p);
            else if (p is double) Stack.Push((double)p1 + (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 + (decimal)p);
            else BasePanic.Throw("Unsupported operation `Add` by type `" + p1.GetType() + "`", 15, "Runtime");
        }

        /// <summary>
        /// Subs last element with second last and pushes it onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Sub(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Unperformable operation `Sub` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 33, "Runtime");
            if (p is byte) Stack.Push((byte)p1 - (byte)p);
            else if (p is short) Stack.Push((short)p1 + (short)p);
            else if (p is int) Stack.Push((int)p1 - (int)p);
            else if (p is long) Stack.Push((long)p1 - (long)p);
            else if (p is float) Stack.Push((float)p1 - (float)p);
            else if (p is double) Stack.Push((double)p1 - (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 - (decimal)p);
            else BasePanic.Throw("Unsupported operation `Sub` by type `" + p1.GetType() + "`", 32, "Runtime");
        }

        /// <summary>
        /// Divides last element with second last and pushes it onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Div(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Tryed to perform operation `Div` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 26, "Runtime");
            if (p is byte) Stack.Push((byte)p1 / (byte)p);
            else if (p is short) Stack.Push((short)p1 / (short)p);
            else if (p is int) Stack.Push((int)p1 / (int)p);
            else if (p is long) Stack.Push((long)p1 / (long)p);
            else if (p is float) Stack.Push((float)p1 / (float)p);
            else if (p is double) Stack.Push((double)p1 / (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 / (decimal)p);
            else BasePanic.Throw("Unsupported operation `Div` by type `" + p1.GetType() + "`", 27, "Runtime");
        }

        /// <summary>
        /// Multiplies last element with second last and pushes it onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Mul(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Tryed to perform operation `Mul` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 28, "Runtime");
            if (p is byte) Stack.Push((byte)p1 * (byte)p);
            else if (p is short) Stack.Push((short)p1 * (short)p);
            else if (p is int) Stack.Push((int)p1 * (int)p);
            else if (p is long) Stack.Push((long)p1 * (long)p);
            else if (p is float) Stack.Push((float)p1 * (float)p);
            else if (p is double) Stack.Push((double)p1 * (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 * (decimal)p);
            else BasePanic.Throw("Unsupported operation `Mul` by type `" + p1.GetType() + "`", 29, "Runtime");
        }

        /// <summary>
        /// Prints the last element onto the stack into the console
        /// </summary>
        /// <param name="args"></param>
        public static void RvmOutput(object args) => Console.Write(Stack.Pop());

        /// <summary>
        /// Asks for input and returns onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void RvmInput(object args) => Stack.Push(Console.ReadLine());

        /// <summary>
        /// Calls shell using the last element onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void RvmShell(object args) => Process.Start(new ProcessStartInfo() { FileName = "cmd", Arguments = "/C " + Stack.Pop<string>(), UseShellExecute = false });

        /// <summary>
        /// Calls a built in rvm function reference stored onto the stack
        /// </summary>
        /// <param name="args"></param>
        public static void RvmCall(object args) => Stack.Pop().Cast<CallPointer>()();

        /// <summary>
        /// Exits from the program and returns stack pop as code
        /// </summary>
        /// <param name="args"></param>
        public static void RvmExit(object args) => Environment.Exit(Stack.Pop().Cast<int>());

        /// <summary>
        /// Throws a new exception with a string parameter as message
        /// </summary>
        /// <param name="args"></param>
        public static void RvmThrow(object args)
        {
            BasePanic.Throw(Stack.Pop().Cast<CacheTable>());
        }

        /// <summary>
        /// Pops the last element of the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Unload(object args) => Stack.PopNR();

        /// <summary>
        /// Dupplicates the last element of the stack
        /// </summary>
        /// <param name="args"></param>
        public static void Duplicate(object args) => Stack.Push(Stack.Peek());

        /// <summary>
        /// Breaks function executing returning to previous
        /// </summary>
        /// <param name="args"></param>
        public static void Return(object args) => ProgramCounter = int.MaxValue - 1;

        /// <summary>
        /// Compares last two elements onto the stack and pushes true if are equals or false
        /// </summary>
        /// <param name="args"></param>
        public static void CompareEQ(object args) => Stack.Push(Stack.Pop().Equals(Stack.Pop()));

        /// <summary>
        /// Compares last two elements onto the stack and pushes true if last is greater than second last or false
        /// </summary>
        /// <param name="args"></param>
        public static void CompareGreater(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Tryed to perform operation `CompareGreater` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 28, "Runtime");
            if (p is byte) Stack.Push((byte)p1 > (byte)p);
            else if (p is short) Stack.Push((short)p1 > (short)p);
            else if (p is int) Stack.Push((int)p1 > (int)p);
            else if (p is long) Stack.Push((long)p1 > (long)p);
            else if (p is float) Stack.Push((float)p1 > (float)p);
            else if (p is double) Stack.Push((double)p1 > (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 > (decimal)p);
            else BasePanic.Throw("Unsupported operation `CompareGreater` by type `" + p1.GetType() + "`", 29, "Runtime");
        }

        /// <summary>
        /// Compares last two elements onto the stack and pushes true if last is less than second last or false
        /// </summary>
        /// <param name="args"></param>
        public static void CompareLess(object args)
        {
            object p = Stack.Pop();
            object p1 = Stack.Pop();
            if (p.GetType() != p1.GetType())
                BasePanic.Throw("Tryed to perform operation `CompareLess` between types `" + p1.GetType() + "` & `" + p.GetType() + "`", 28, "Runtime");
            if (p is byte) Stack.Push((byte)p1 < (byte)p);
            else if (p is short) Stack.Push((short)p1 < (short)p);
            else if (p is int) Stack.Push((int)p1 < (int)p);
            else if (p is long) Stack.Push((long)p1 < (long)p);
            else if (p is float) Stack.Push((float)p1 < (float)p);
            else if (p is double) Stack.Push((double)p1 < (double)p);
            else if (p is decimal) Stack.Push((decimal)p1 < (decimal)p);
            else BasePanic.Throw("Unsupported operation `CompareLess` by type `" + p1.GetType() + "`", 29, "Runtime");
        }

        /// <summary>
        /// Compares last two elements onto the stack and pushes true if are not equals or false
        /// </summary>
        /// <param name="args"></param>
        public static void CompareNEQ(object args) => Stack.Push(!Stack.Pop().Equals(Stack.Pop()));

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if true
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void JumpTrue(object args)
        {
            if (Stack.Pop().Cast<bool>())
                ProgramCounter = (int)args - 1;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void JumpFalse(object args)
        {
            if (!Stack.Pop().Cast<bool>())
                ProgramCounter = (int)args - 1;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if true
        /// </summary>
        /// <param name="args">Name of the label to jump on</param>
        public static void JumpTrueLabel(object args)
        {
            if (Stack.Pop().Cast<bool>())
                ProgramCounter = CurrentFunctionPointer.FindLabel((string)args) - 1;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Name of the label to jump on</param>
        public static void JumpFalseLabel(object args)
        {
            if (!Stack.Pop().Cast<bool>())
                ProgramCounter = CurrentFunctionPointer.FindLabel((string)args) - 1;
        }


        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void Skip(object args)
        {
            ProgramCounter += (int)args;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void SkipTrue(object args)
        {
            if (Stack.Pop().Cast<bool>())
                ProgramCounter += (int)args;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void SkipFalse(object args)
        {
            if (!Stack.Pop().Cast<bool>())
                ProgramCounter += (int)args;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void BackTrue(object args)
        {
            if (Stack.Pop().Cast<bool>())
                ProgramCounter -= (int)args;
        }

        /// <summary>
        /// Pops last element of the stack and jump to <paramref name="args"/> if false
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void BackFalse(object args)
        {
            if (!Stack.Pop().Cast<bool>())
                ProgramCounter -= (int)args;
        }

        /// <summary>
        /// Jumps to <paramref name="args"/>
        /// </summary>
        /// <param name="args">Index of instruction to jump on</param>
        public static void Jump(object args) => ProgramCounter = (int)args - 1;

        /// <summary>
        /// Jumps to <paramref name="args"/>
        /// </summary>
        /// <param name="args">Name of the label to jump on</param>
        public static void JumpLabel(object args) => ProgramCounter = CurrentFunctionPointer.FindLabel((string)args) - 1;
    }
}