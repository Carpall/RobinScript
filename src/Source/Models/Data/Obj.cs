using System.Collections.Generic;

namespace RobinVM.Models
{
    public struct Obj
    {
        public CacheTable CacheTable;
        public Function? Ctor;
        public Obj Copy() => new Obj { CacheTable = CacheTable.Clone(), Ctor = Ctor.Value };
    }
}