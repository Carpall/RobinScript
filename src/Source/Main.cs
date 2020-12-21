using RobinVM;
using RobinVM.Models;

var main = Function.New
(
    Instruction.New(Runtime.Load, false),
    Instruction.New(Runtime.Load, false),
    Instruction.New(Runtime.And),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);

var image = Image.New("main", ref main);

image.Execute();
