using RobinVM;
using RobinVM.Models;

var main = Function.New
(
    Instruction.New(Runtime.Load, "Hello1"),
    Instruction.New(Runtime.Load, "Hello2"),
    Instruction.New(Runtime.Load, "Hello3"),
    Instruction.New(Runtime.LoadVector, 3),
    Instruction.New(Runtime.CallInstance, "tostr()"),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);

var image = Image.New("main", ref main);

image.Execute();
