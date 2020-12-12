using System;
using System.Collections.Generic;
using System.Text;

namespace RobinVM.Models.BuiltIn
{
    public static class Extensions
    {
        // Vec
        public static void SysVec_GetValue(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()]);
        }
        public static void SysVec_SetValue(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()] = Runtime.CurrentFunctionPointer.FindArgument(2);
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Clear(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>().Clear();
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Find(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>().Contains(Runtime.CurrentFunctionPointer.FindArgument(1)));
        }
        public static void SysVec_Len(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>().Count);
        }
        public static void SysVec_Clone(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>().Clone());
        }
        public static void SysVec_ToString(object nill = null)
        {
            Runtime.Stack.Push("["+string.Join(", ", Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>())+"]");
        }
        public static void SysVec_Concat(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>().AddRange(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<CacheTable>()["ptr"].Cast<List<object>>());
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Insert(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<List<object>>().Insert(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>(), Runtime.CurrentFunctionPointer.FindArgument(2));
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }


        // Str
        public static void SysStr_GetValue(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()]);
        }
        public static void SysStr_Clear(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>().Set("ptr", "");
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysStr_Find(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>().Contains(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<string>()));
        }
        public static void SysStr_Len(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>().Length);
        }
        public static void SysStr_Clone(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>().Clone());
        }
        public static void SysStr_Concat(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>() + Runtime.CurrentFunctionPointer.FindArgument(1).Cast<CacheTable>()["ptr"].Cast<string>());
        }
        public static void SysStr_ToString(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>());
        }
        public static void SysStr_Insert(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>().Insert(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>(), Runtime.CurrentFunctionPointer.FindArgument(2).Cast<string>());
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysStr_ParseNum(object nill = null)
        {
            Runtime.Stack.Push(int.Parse(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<CacheTable>()["ptr"].Cast<string>()));
        }
    }
}


namespace RobinVM.Models
{
    public partial struct Image
    {
        public void InitializeBuiltIn()
        {
            // Print
            CacheTable.Add("print",
                Function.New(
                    Instruction.New(Runtime.LoadFromArgs, 0),
                    Instruction.New(Runtime.CallInstance, "tostr()"),
                    Instruction.New(Runtime.RvmOutput),
                    Instruction.New(Runtime.Return)
                ));

            CacheTable.Add("println",
            Function.New(
                Instruction.New(Runtime.LoadFromArgs, 0),
                Instruction.New(Runtime.CallInstance, "tostr()"),
                Instruction.New(Runtime.RvmOutput),
                Instruction.New(Runtime.Stack.Push, '\n'),
                Instruction.New(Runtime.RvmOutput),
                Instruction.New(Runtime.Return)
            ));


            // BasePanic
            CacheTable.Add("basepanic",
                new Obj
                {
                    Ctor = Function.New
                    (
                        Instruction.New(Runtime.LoadFromArgs, 0),
                        Instruction.New(Runtime.LoadFromArgs, 1),
                        Instruction.New(Runtime.StoreGlobal, "msg"),
                        Instruction.New(Runtime.LoadFromArgs, 0),
                        Instruction.New(Runtime.LoadFromArgs, 2),
                        Instruction.New(Runtime.StoreGlobal, "code"),
                        Instruction.New(Runtime.LoadFromArgs, 0),
                        Instruction.New(Runtime.LoadFromArgs, 3),
                        Instruction.New(Runtime.StoreGlobal, "type"),
                        Instruction.New(Runtime.Return)
                    ),
                    CacheTable = new CacheTable(new Dictionary<string, object>()
                    {
                        { "$", "basepanic" },
                        { "msg", null },
                        { "code", null },
                        { "type", null },
                        { "throw()",
                            Function.New
                            (
                                Instruction.New(Runtime.LoadFromArgs, 0),
                                Instruction.New(Runtime.RvmThrow),
                                Instruction.New(Runtime.Return)
                            )
                        }
                    })
                });

            // Str
            CacheTable.Add("str",
                new Obj
                {
                    Ctor = Function.New
                    (
                        Instruction.New(Runtime.Return)
                    ),
                    CacheTable = new CacheTable(new Dictionary<string, object>()
                    {
                        { "$", "str" },
                        { "ptr", null },
                        { "tostr()",
                            Function.New
                            (
                                Instruction.New(Runtime.LoadFromArgs, 0),
                                Instruction.New(Runtime.LoadGlobal, "ptr"),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "tonum()",
                            Function.New
                            (
                                Instruction.New(Runtime.LoadFromArgs, 0),
                                Instruction.New(Runtime.LoadGlobal, "ptr"),
                                Instruction.New(Runtime.Cast<float>),
                                Instruction.New(Runtime.Return)
                            )
                        }
                    })
                });
        }
    }
}
