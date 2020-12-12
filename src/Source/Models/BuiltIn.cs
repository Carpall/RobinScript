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
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()]);
        }
        public static void SysVec_SetValue(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()] = Runtime.CurrentFunctionPointer.FindArgument(2);
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Clear(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>().Clear();
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Find(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>().Contains(Runtime.CurrentFunctionPointer.FindArgument(1)));
        }
        public static void SysVec_Len(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>().Count);
        }
        public static void SysVec_Clone(object nill = null)
        {
            Runtime.Stack.Push(new Dictionary<string, object>(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()));
        }
        public static void SysVec_ToString(object nill = null)
        {
            Runtime.Stack.Push("["+string.Join(", ", Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>())+"]");
        }
        public static void SysVec_Concat(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>().AddRange(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>());
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysVec_Insert(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<List<object>>().Insert(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>(), Runtime.CurrentFunctionPointer.FindArgument(2));
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }


        // Str
        public static void SysStr_GetValue(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>()[Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>()]);
        }
        public static void SysStr_Clear(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"] = "";
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysStr_Find(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>().Contains(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<string>()));
        }
        public static void SysStr_Len(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>().Length);
        }
        public static void SysStr_Clone(object nill = null)
        {
            Runtime.Stack.Push(new Dictionary<string, object>(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()));
        }
        public static void SysStr_Concat(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>() + Runtime.CurrentFunctionPointer.FindArgument(1).Cast<Dictionary<string, object>>()["ptr"].Cast<string>());
        }
        public static void SysStr_ToString(object nill = null)
        {
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>());
        }
        public static void SysStr_Insert(object nill = null)
        {
            Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>().Insert(Runtime.CurrentFunctionPointer.FindArgument(1).Cast<int>(), Runtime.CurrentFunctionPointer.FindArgument(2).Cast<string>());
            Runtime.Stack.Push(Runtime.CurrentFunctionPointer.FindArgument(0));
        }
        public static void SysStr_ParseNum(object nill = null)
        {
            Runtime.Stack.Push(int.Parse(Runtime.CurrentFunctionPointer.FindArgument(0).Cast<Dictionary<string, object>>()["ptr"].Cast<string>()));
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
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::basepanic" },
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
                    }
                });
            CacheTable.Add("vec",
                new Obj
                {
                    Ctor = Function.New
                    (
                        Instruction.New(Runtime.Return)
                    ),
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::vec" },
                        { "ptr", null },
                        { "len()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Len),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "get(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_GetValue),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "set(..)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_SetValue),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "clear()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Clear),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "find(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Find),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "clone()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Clone),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "getstr()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_ToString),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "add(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Concat),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "add(..)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysVec_Insert),
                                Instruction.New(Runtime.Return)
                            )
                        },
                    }
                });
            CacheTable.Add("str",
                new Obj
                {
                    Ctor = Function.New
                    (
                        Instruction.New(Runtime.Return)
                    ),
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::str" },
                        { "ptr", null },
                        { "len()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Len),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "get(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_GetValue),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "clear()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Clear),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "find(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Find),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "clone()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Clone),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "getstr()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_ToString),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "add(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Concat),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "add(..)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Insert),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "parse()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_ParseNum),
                                Instruction.New(Runtime.Return)
                            )
                        },
                    }
                });
            CacheTable.Add("num",
                new Obj
                {
                    Ctor = Function.New
                    (
                        Instruction.New(Runtime.Return)
                    ),
                    CacheTable = new Dictionary<string, object>()
                    {
                        { "$", "sys::num" },
                        { "ptr", null },
                        { "set(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Clone),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "clone()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Clone),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "getstr()",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_ToString),
                                Instruction.New(Runtime.Return)
                            )
                        },
                        { "add(.)",
                            Function.New
                            (
                                Instruction.New(BuiltIn.Extensions.SysStr_Concat),
                                Instruction.New(Runtime.Return)
                            )
                        },
                    }
                });
        }
    }
}
