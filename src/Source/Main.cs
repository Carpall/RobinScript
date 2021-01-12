using RobinVM;
using RobinVM.Models;

var main = Function.New
(   0,
    Instruction.New(Runtime.Load, "arg0"),
    Instruction.New(Runtime.Load, "arg1"),
    Instruction.New(Runtime.Call, "f(.)"),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);
var f = Function.New
(
    1,
    Instruction.New(Runtime.LoadFromArgs, 0),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);

var image = Image.New("main", ref main);

image.AddFunction("f(.)", f);
image.Execute();
