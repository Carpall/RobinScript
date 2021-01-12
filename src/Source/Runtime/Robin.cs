using RobinVM.Models;
using System;
using System.Collections.Generic;

namespace RobinVM
{
    public static class Robin
    {
        /// <summary>
        /// Performs main function of <paramref name="program"/>
        /// <code>
        /// <see cref="Instruction"/>[] main = {<br/>
        /// ••• <see cref="Instruction"/>.New(<see cref="Runtime"/>.Load, "Hello World!"),<br/>
        /// ••• <see cref="Instruction"/>.New(<see cref="Runtime"/>.RvmOutput),<br/>
        /// ••• <see cref="Instruction"/>.New(<see cref="Runtime"/>.Return),<br/>
        /// };<br/>
        /// <see cref="Models.Function"/>[] program = {<br/>
        /// ••• <see cref="Models.Function"/>.New(main, "main")<br/>
        /// };<br/>
        /// <see cref="Robin"/>.Execute(program);
        /// </code>
        /// </summary>
        /// <param name="program">Functions</param>
        public static void Execute(this Image programImage)
        {
            programImage.InitializeBuiltIn();
            Runtime.Storage = new object[byte.MaxValue];
            Runtime.RuntimeImage = programImage;
            programImage.EntryPointPointer.ExecuteLabel(programImage.ManifestName);
        }
        public static void ExecuteLabel(this Function function, string trail)
        {
            var x1 = Runtime.ProgramCounter;
            BasePanic.LoadTrail(trail);
            Runtime.Stack.TransferToArguments(ref function);
            var x3 = Runtime.Stack;
            Runtime.Stack = new RStack(1000);
            Runtime.CurrentFunctionPointer = function;
            if (Runtime.StorageManager != 0)
            {
                var x2 = Runtime.StorageManager;
                var x0 = Runtime.Storage;
                Runtime.Storage = new object[byte.MaxValue];

                for (Runtime.ProgramCounter = 0, Runtime.StorageManager = 0; Runtime.ProgramCounter < function.Instructions.Length; Runtime.ProgramCounter++)
                    function.Instructions[Runtime.ProgramCounter].FunctionPointer(function.Instructions[Runtime.ProgramCounter].Argument);

                Runtime.Storage = x0;
                Runtime.StorageManager = x2;
            }
            else
            {
                for (Runtime.ProgramCounter = 0, Runtime.StorageManager = 0; Runtime.ProgramCounter < function.Instructions.Length; Runtime.ProgramCounter++)
                    function.Instructions[Runtime.ProgramCounter].FunctionPointer(function.Instructions[Runtime.ProgramCounter].Argument);

                if (Runtime.StorageManager != 0)
                    Array.Clear(Runtime.Storage, byte.MinValue, byte.MaxValue);
            }
            Runtime.Stack = x3;
            BasePanic.UnloadTrail();
            Runtime.ProgramCounter = x1;
        }
    }
}