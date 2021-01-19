using RobinVM;
using RobinVM.Models;

var main = Function.New
(
    0,
    Instruction.New(Runtime.NewObj, "Person"),
    Instruction.New(Runtime.LoadGlobal, "ag"),
    Instruction.New(Runtime.RvmOutput),
    Instruction.New(Runtime.Return)
);

var image = Image.New("main", ref main);

var person = new Obj() { CacheTable = new CacheTable() };
person.Ctor = Function.New
(
    0,
    Instruction.New(Runtime.Return)
);
person.CacheTable.Add("$", "Person");
person.CacheTable.Add("name", "");
person.CacheTable.Add("age", 0);

image.AddObj("Person", person);

image.Execute();