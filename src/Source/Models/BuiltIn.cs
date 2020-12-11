using System;
using System.Collections.Generic;
using System.Text;

namespace RobinVM.Models.BuiltIn
{
    public static class Extensions
    {
        public static void SysVec_GetValue(object nill = null)
        {
            Runtime.Stack.Push(((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()]);
        }
        public static void SysVec_SetValue(object nill = null)
        {
            ((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()] = Runtime.CurrentFunctionPointer.FindArgument(2);
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Clear(object nill = null)
        {
            (((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>()).Clear();
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Find(object nill = null)
        {
            Runtime.Stack.Push((((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>()).Contains(Runtime.CurrentFunctionPointer.FindArgument(1)));
        }
        public static void SysVec_Len(object nill = null)
        {
            Runtime.Stack.Push(((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>().Count);
        }
        public static void SysVec_Clone(object nill = null)
        {
            Runtime.Stack.Push(new List<object>((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"].Cast<List<object>>()));
        }
        public static void SysVec_ToString(object nill = null)
        {
            Runtime.Stack.Push("["+string.Join(", ", ((Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>())["arr"]).Cast<List<object>>())+"]");
        }
        public static void SysVec_Ctor(object nill = null)
        {
            Runtime.LoadFromArgs(0);
            Runtime.CurrentFunctionPointer.LoadArgumentsAsList();
            Runtime.StoreGlobal("arr");
        }
    }
}


namespace RobinVM.Models
{
    public partial struct Image
    {
        public void InitializeBuiltIn()
        {
            CacheTable.Add("basepanic",
                new Obj
                {
                    Ctor = Function.New
                    (
                        new Instruction[]
                        {
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
                        }
                    ),
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::basepanic" },
                        { "msg", null },
                        { "code", null },
                        { "type", null },
                        { "throw()",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(Runtime.LoadFromArgs, 0),
                                    Instruction.New(Runtime.RvmThrow),
                                    Instruction.New(Runtime.Return),
                                }
                            )
                        }
                    }
                });

            CacheTable.Add("vec",
                new Obj
                {
                    Ctor = Function.New
                    (
                        new Instruction[]
                        {
                            Instruction.New(BuiltIn.Extensions.SysVec_Ctor),
                            Instruction.New(Runtime.Return)
                        }
                    ),
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::vec" },
                        { "arr", null },
                        { "len()",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_Len),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "get(.)",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_GetValue),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "set(..)",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_SetValue),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "clear()",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_Clear),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "find(.)",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_Find),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "clone()",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_Clone),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                        { "getstr()",
                            Function.New
                            (
                                new Instruction[]
                                {
                                    Instruction.New(BuiltIn.Extensions.SysVec_ToString),
                                    Instruction.New(Runtime.Return)
                                }
                            )
                        },
                    }
                });
        }
    }
}
