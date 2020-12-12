using RobinVM;
using RobinVM.Models;

var main = Function.New
(
    Instruction.New(Runtime.LoadString, "Hello"),
    Instruction.New(Runtime.Store, 0),
    Instruction.New(Runtime.LoadString, ""),
    Instruction.New(Runtime.Store, 0),
    Instruction.New(Runtime.LoadFromStorage, 0),
    Instruction.New(Runtime.CallInstance, "getstr()"),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);

var image = Image.New("main", ref main);

image.Execute(); 