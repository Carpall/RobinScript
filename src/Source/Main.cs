using RobinVM;
using RobinVM.Models;

class Test
{
    static void Main()
    {
        var main = Function.New
        (
            Instruction.New(Runtime.Load, "first"), // load vector elements
            Instruction.New(Runtime.Load, "second"),
            Instruction.New(Runtime.Load, "tird"),
            Instruction.New(Runtime.NewObj, "vec"), // instantiate a new vector
            Instruction.New(Runtime.Load, "index here"), // index to load
            Instruction.New(Runtime.CallInstance, "get(.)"), // get element from index
                                                              // &: instance, .: first argument
            Instruction.New(Runtime.RvmOutput), // output the element
            Instruction.New(Runtime.Return)
        );

        var image = Image.New(manifestName: "main", entryPoint: ref main);

        image.Execute();

    }
}